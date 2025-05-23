import React from 'react';
import { EntityHistory } from '../types/ComponentStatus';

type TimelineBlocksProps = {
    history: EntityHistory;
    blockSize: 15 | 30 | 60; // minutes
};

const TimelineBlocks: React.FC<TimelineBlocksProps> = ({ history, blockSize }) => {
    const handleMouseMove = (e: React.MouseEvent<HTMLDivElement>) => {
        const tooltip = e.currentTarget.querySelector('.timeline-tooltip-wrapper') as HTMLElement;
        if (tooltip) {
            tooltip.style.left = `${e.clientX}px`;
            tooltip.style.top = `${e.clientY}px`;
        }
    };
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

    const getWorstStatus = (statuses: string[]): string => {
        if (statuses.includes('Unhealthy')) return 'Unhealthy';
        if (statuses.includes('Degraded')) return 'Degraded';
        if (statuses.includes('Healthy')) return 'Healthy';
        return 'Unknown';
    };

    const getLast24HoursBlocks = () => {
        const now = new Date();
        const blocks: { startTime: Date; endTime: Date; status: string }[] = [];

        // Generate blocks for the last 24 hours
        for (let i = 0; i < (24 * 60) / blockSize; i++) {
            const blockEndTime = new Date(now.getTime() - (i * blockSize * 60 * 1000));
            const blockStartTime = new Date(blockEndTime.getTime() - (blockSize * 60 * 1000));
            
            // Find all states that occurred during this block's time window
            const statesInBlock: string[] = [];
            
            // Add the state from the most recent transition before the block
            let initialState = 'Unknown';
            for (let j = history.history.transitions.length - 1; j >= 0; j--) {
                const transition = history.history.transitions[j];
                const transitionTime = new Date(transition.occurrenceTimeUtc);
                if (transitionTime <= blockStartTime) {
                    initialState = transition.newState;
                    break;
                }
            }
            statesInBlock.push(initialState);

            // Add states from all transitions within the block's time window
            for (const transition of history.history.transitions) {
                const transitionTime = new Date(transition.occurrenceTimeUtc);
                if (transitionTime >= blockStartTime && transitionTime <= blockEndTime) {
                    statesInBlock.push(transition.previousState);
                    statesInBlock.push(transition.newState);
                }
            }

            // Get the worst status from all states in this block
            const worstStatus = getWorstStatus(statesInBlock);
            blocks.unshift({ startTime: blockStartTime, endTime: blockEndTime, status: worstStatus });
        }

        return blocks;
    };

    const blocks = getLast24HoursBlocks();

    return (
        <div className="timeline-wrapper">
            <style>
                {`
                    .timeline-wrapper {
                        position: relative;
                        width: 100%;
                        overflow: hidden;
                    }
                    .timeline-blocks {
                        display: flex;
                        gap: 2px;
                        height: 24px;
                        border-radius: 4px;
                        position: relative;
                        isolation: isolate;
                        overflow-x: auto;
                        overflow-y: hidden;
                        white-space: nowrap;
                        padding: 0 16px 6px;
                        scrollbar-width: thin;
                        scrollbar-color: rgba(155, 155, 155, 0.5) transparent;
                    }
                    
                    .timeline-blocks::-webkit-scrollbar {
                        height: 6px;
                    }
                    
                    .timeline-blocks::-webkit-scrollbar-track {
                        background: transparent;
                    }
                    
                    .timeline-blocks::-webkit-scrollbar-thumb {
                        background-color: rgba(155, 155, 155, 0.5);
                        border-radius: 3px;
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
                        position: fixed;
                        pointer-events: none;
                        width: max-content;
                        z-index: 99999;
                    }
                    .timeline-tooltip {
                        background: rgba(0, 0, 0, 0.9);
                        position: relative;
                        transform: translate(-50%, -100%);
                        margin-top: -10px;
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
            <div className="timeline-blocks">
                {blocks.map((block, index) => (
                    <div
                        key={index}
                        className="timeline-block"
                        style={{ backgroundColor: getStatusColor(block.status) }}
                        onMouseMove={handleMouseMove}
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
        </div>
    );
};

export default TimelineBlocks;
