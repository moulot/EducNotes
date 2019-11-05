export interface Period {
    id: number;
    name: string;
    abbrev: string;
    startDate?: Date;
    endDate?: Date;
    active: boolean;
}
