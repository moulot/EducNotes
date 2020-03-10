export interface UserClassEvent {
  id: number;
  userId: number;
  classEventId: number;
  doneById: number;
  periodId: number;
  startDate: Date;
  endDate: Date;
  justified: boolean;
  reason: string;
  comment: string;

}