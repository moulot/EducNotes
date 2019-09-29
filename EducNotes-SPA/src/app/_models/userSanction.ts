export interface UserSanction {
    userName: string;
    id: number;
    userId: number;
    sanctionId: number;
    sanctionedById: number;
    sanctionDate: Date;
    reason: string;
    comment: string;
}
