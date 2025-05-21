export type ComponentStatus = {
    name: string;
    status: 'Operational' | 'Degraded' | 'Outage';
    description: string;
    lastUpdated: string;
};
