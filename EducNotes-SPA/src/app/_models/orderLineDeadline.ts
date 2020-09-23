export interface OrderLineDeadline {
  id: number;
  orderLineId: number;
  percent: number;
  amount: number;
  productFee: number;
  dueDate: Date;
  strDueDate: string;
  deadlineName: string;
  comment: string;
  seq: number;
}
