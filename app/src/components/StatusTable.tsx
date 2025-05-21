import React from 'react';
import { ComponentStatus } from '../types/ComponentStatus';
import StatusIndicator from './StatusIndicator';

type StatusTableProps = {
    components: ComponentStatus[];
};

const StatusTable: React.FC<StatusTableProps> = ({ components }) => {
    return (
        <div className="status-table">
            <style>
                {`
                    .status-table {
                        background: white;
                        border-radius: 8px;
                        box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
                        overflow: hidden;
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
                    }
                    .status-table .description {
                        color: #666;
                    }
                    .status-table .timestamp {
                        color: #888;
                        font-size: 0.9em;
                    }
                `}
            </style>
            <table>
                <thead>
                    <tr>
                        <th>Component</th>
                        <th>Status</th>
                        <th>Description</th>
                        <th>Last Updated</th>
                    </tr>
                </thead>
                <tbody>
                    {components.map((component, index) => (
                        <tr key={index}>
                            <td className="component-name">{component.name}</td>
                            <td>
                                <div style={{ display: 'flex', alignItems: 'center' }}>
                                    <StatusIndicator status={component.status} />
                                    {component.status}
                                </div>
                            </td>
                            <td className="description">{component.description}</td>
                            <td className="timestamp">
                                {new Date(component.lastUpdated).toLocaleString()}
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
};

export default StatusTable;
