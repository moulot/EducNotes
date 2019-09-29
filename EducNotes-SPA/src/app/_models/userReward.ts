export interface UserReward {
    id: number;
    userId: number;
    rewardId: number;
    rewardedById: number;
    rewardDate: Date;
    reason: string;
    comment: string;
}
