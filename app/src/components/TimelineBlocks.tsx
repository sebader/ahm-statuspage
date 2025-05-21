import React from 'react';
import { EntityHistory } from '../types/ComponentStatus';

type TimelineBlocksProps = {
    history: EntityHistory;
    blockSize: 30 | 60; // minutes
};

const TimelineBlocks: React.FC<TimelineBlocksProps> = ({ history, blockSize }) => {
    const getStatusColor = (status: string): string => {
        switch (status) {
            case 'Healthy':
                return '#22c55e';
            case 'Degraded':
                return '#f59e0b';
            case 'Unhealthy':
                return '#ef4444';
            default:
                return '#94a3b8';
        }
    };

    const getLast24HoursBlocks = () => {
        const now = new Date();
        const blocks: { startTime: Date; endTime: Date; status: string }[] = [];
        const lastTransition = history.history.transitions[history.history.transitions.length - 1];
        let currentStatus = lastTransition.newState;

        // Generate blocks for the last 24 hours
        for (let i = 0; i < (24 * 60) / blockSize; i++) {
            const blockTime = new Date(now.getTime() - (i * blockSize * 60 * 1000));
            
            // Find the status at this time by going through transitions backwards
            for (let j = history.history.transitions.length - 1; j >= 0; j--) {
                const transition = history.history.transitions[j];
                const transitionTime = new Date(transition.occurrenceTimeUtc);
                
                if (blockTime >= transitionTime) {
                    currentStatus = transition.newState;
                    break;
                }
            }
            
            const blockEndTime = new Date(blockTime.getTime() + (blockSize * 60 * 1000));
            blocks.unshift({ startTime: blockTime, endTime: blockEndTime, status: currentStatus });
        }

        return blocks;
    };

    const blocks = getLast24HoursBlocks();

    return (
        <div className="timeline-blocks">
            <style>
                {`
                    .timeline-blocks {
                        display: flex;
                        gap: 2px;
                        height: 24px;
                        border-radius: 4px;
                        overflow: visible;
                        position: relative;
                    }
                    .timeline-block {
                        flex: 1;
                        height: 100%;
                        transition: transform 0.2s;
                        position: relative;
                        cursor: pointer;
                    }
                    .timeline-block:hover {
                        background-color: rgba(255, 255, 255, 0.1);
                    }
                    .timeline-tooltip-wrapper {
                        position: absolute;
                        bottom: calc(100% + 10px);
                        left: 50%;
                        transform: translateX(-50%);
                        pointer-events: none;
                        width: max-content;
                    }
                    .timeline-tooltip {
                        background: rgba(0, 0, 0, 0.9);
                        color: white;
                        padding: 8px 12px;
                        border-radius: 6px;
                        font-size: 12px;
                        white-space: nowrap;
                        z-index: 9999;
                        box-shadow: 0 4px 8px rgba(0,0,0,0.3);
                        text-align: center;
                        opacity: 0;
                        visibility: hidden;
                        transition: opacity 0.2s, visibility 0.2s;
                    }
                    .timeline-block:hover .timeline-tooltip {
                        opacity: 1;
                        visibility: visible;
                    }
                `}
            </style>
                {blocks.map((block, index) => (
                <div
                    key={index}
                    className="timeline-block"
                    style={{ backgroundColor: getStatusColor(block.status) }}
                >
                    <div className="timeline-tooltip-wrapper">
                        <div className="timeline-tooltip">
                            <div style={{ fontWeight: 'bold', marginBottom: '4px' }}>{block.status}</div>
                            <div>{block.startTime.toLocaleDateString()}</div>
                            <div>{block.startTime.toLocaleTimeString()} - {block.endTime.toLocaleTimeString()}</div>
                        </div>
                    </div>
                </div>
            ))}
        </div>
    );
};

export default TimelineBlocks;
