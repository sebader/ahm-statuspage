import React, { useState, useEffect } from 'react';
import { ComponentStatus, EntityHistory } from '../types/ComponentStatus';
import StatusIndicator from './StatusIndicator';
import TimelineBlocks from './TimelineBlocks';

type StatusTableProps = {
    components: ComponentStatus[];
};

const StatusTable: React.FC<StatusTableProps> = ({ components }) => {
    const [entityHistories, setEntityHistories] = useState<Record<string, EntityHistory>>({});
    const [loadingHistory, setLoadingHistory] = useState<Record<string, boolean>>({});

    useEffect(() => {
        const fetchEntityHistory = async (entityName: string) => {
            try {
                setLoadingHistory(prev => ({ ...prev, [entityName]: true }));
                const response = await fetch(`/api/history/${encodeURIComponent(entityName)}`);
                if (!response.ok) throw new Error('Failed to fetch history');
                const history: EntityHistory = await response.json();
                setEntityHistories(prev => ({ ...prev, [entityName]: history }));
            } catch (error) {
                console.error('Error fetching history:', error);
            } finally {
                setLoadingHistory(prev => ({ ...prev, [entityName]: false }));
            }
        };

        components.forEach(component => {
            fetchEntityHistory(component.name);
        });
    }, [components]);

    return (
        <div className="status-table">
            <style>
                {`
                    .status-table {
                        background: white;
                        border-radius: 8px;
                        box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
                        position: relative;
                    }
                    .status-table table {
                        width: 100%;
                        border-collapse: collapse;
                    }
                    .status-table th {
                        background: #f8f9fa;
                        padding: 16px;
                        text-align: left;
                        font-weight: 600;
                        color: #444;
                        border-bottom: 1px solid #eee;
                    }
                    .status-table td {
                        padding: 16px;
                        border-bottom: 1px solid #eee;
                    }
                    .status-table tr:last-child td {
                        border-bottom: none;
                    }
                    .status-table .component-name {
                        font-weight: 500;
                        color: #2c3e50;
                        display: flex;
                        align-items: center;
                    }
                    .status-table .overall-status {
                        font-weight: 600;
                        color: #1a202c;
                    }
                    .status-table .overall-status-indicator {
                        font-weight: 600;
                    }
                    .status-table .indented-status {
                        padding-left: 24px;
                    }
                    .status-table .description {
                        color: #666;
                    }
                    .status-table .timestamp {
                        color: #888;
                        font-size: 0.9em;
                    }
                    .loading-placeholder {
                        color: #666;
                        font-style: italic;
                        background: linear-gradient(90deg, #f0f0f0 25%, #e0e0e0 50%, #f0f0f0 75%);
                        background-size: 200% 100%;
                        animation: loading 1.5s infinite;
                        border-radius: 4px;
                        height: 24px;
                    }
                    @keyframes loading {
                        0% { background-position: 200% 0; }
                        100% { background-position: -200% 0; }
                    }
                `}
            </style>
            <table>
                <thead>
                    <tr>
                        <th>Component</th>
                        <th>Status</th>
                        <th>Last 24 Hours</th>
                    </tr>
                </thead>
                <tbody>
                    {components.map((component, index) => (
                        <tr key={index}>
                            <td className={`component-name ${index === 0 ? 'overall-status' : 'indented-status'}`}>
                                {component.displayName || component.name}
                            </td>
                            <td>
                                <div style={{ display: 'flex', alignItems: 'center' }} className={index === 0 ? 'overall-status-indicator' : ''}>
                                    <StatusIndicator status={component.status} />
                                    {component.status}
                                </div>
                            </td>
                            <td style={{ minWidth: '400px', maxWidth: '400px', position: 'relative', padding: '16px 0' }}>
                                {loadingHistory[component.name] ? (
                                    <div className="loading-placeholder" />
                                ) : entityHistories[component.name] ? (
                                    <TimelineBlocks history={entityHistories[component.name]} blockSize={15} />
                                ) : (
                                    "No history data available"
                                )}
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
};

export default StatusTable;
