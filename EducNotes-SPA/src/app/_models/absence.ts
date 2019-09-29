export interface Absence {
    userName: string;
    id: number;
    userId: number;
    absenceTypeId: number;
    sessionId: number;
    startDate: Date;
    endDate: Date;
    justified: boolean;
    reason: string;
    comment: string;
}
