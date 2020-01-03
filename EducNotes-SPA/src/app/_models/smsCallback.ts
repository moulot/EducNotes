export interface SmsCallback {
    apiMessageId: string;
    accepted: boolean;
    to: string;
    errorCode: string;
    error: number;
    errorDesc: string;
}
