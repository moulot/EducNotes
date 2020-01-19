
export interface clickatellParams {
  content: string;
  to: string;
  from: string;

  binary: Boolean;

  clientMsgId: string;

  scheduledDeliveryTime: Date;

  userDataHeader: string;

  validityPeriod: number;

  charset: string;
}
