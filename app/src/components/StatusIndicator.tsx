import React from 'react';
import { ComponentStatus } from '../types/ComponentStatus';

type StatusIndicatorProps = {
    status: ComponentStatus['status'];
};

const StatusIndicator: React.FC<StatusIndicatorProps> = ({ status }) => {
    const getStatusColor = () => {
        switch (status) {
            case 'Healthy':
                return '#2ecc71'; // green
            case 'Degraded':
                return '#f1c40f'; // yellow
            case 'Unhealthy':
                return '#e74c3c'; // red
            default:
                return '#95a5a6'; // grey
        }
    };

    return (
        <div
            style={{
                width: '12px',
                height: '12px',
                borderRadius: '50%',
                backgroundColor: getStatusColor(),
                display: 'inline-block',
                marginRight: '8px'
            }}
            title={status}
        />
    );
};

export default StatusIndicator;
