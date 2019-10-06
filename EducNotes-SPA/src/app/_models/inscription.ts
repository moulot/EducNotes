export interface Inscription {
    id: number;
    insertDate: Date;
    classLevelId: number;
    userId: number;
    validated: boolean;
    validatedDate: Date;
    inscriptionTypeId: number;
    insertUserId: number;
}
