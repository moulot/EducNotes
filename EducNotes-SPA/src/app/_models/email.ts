export interface Email {
    id: number;
    emailTypeId: number;
    toAddress: string;
    ccAddress: string;
    bccAddress: string;
    fromAddress: string;
    subject: string;
    body: string;
    timeToSend: Date;
    statusFlag: number;
    insertDate: Date;
    insertUserId: number;
    updateDate: Date;
    updateUserId: number;
}
