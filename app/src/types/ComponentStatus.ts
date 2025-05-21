export type StateTransition = {
    previousState: string;
    newState: 'Healthy' | 'Degraded' | 'Unhealthy' | 'Unknown';
    occurrenceTimeUtc: string;
    labels: Record<string, string>;
    eventId: string;
};

export type EntityHistory = {
    entityName: string;
    history: {
        transitions: StateTransition[];
    };
};

export type ComponentStatus = {
    name: string;
    displayName?: string;
    status: 'Healthy' | 'Degraded' | 'Unhealthy' | 'Unknown';
    description: string;
    lastStatusChange: string;
    history?: EntityHistory;
};
