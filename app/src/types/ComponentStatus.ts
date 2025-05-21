export type ComponentStatus = {
    name: string;
    status: 'Healthy' | 'Degraded' | 'Error' | 'Unknown';
    description: string;
    lastUpdated: string;
};
