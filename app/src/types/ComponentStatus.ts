export type ComponentStatus = {
    name: string;
    status: 'Healthy' | 'Degraded' | 'Unhealthy' | 'Unknown';
    description: string;
    lastUpdated: string;
};
