import { TuitionChildData } from './tuitionChildData';

export interface TuitionData {
  fLastName: string;
  fFirstName: string;
  fCell: string;
  fEmail: string;
  fSendEmail: boolean;
  mLastName: string;
  mFirstName: string;
  mCell: string;
  mEmail: string;
  mSendEmail: boolean;
  orderAmount: number;
  dueAmount: number;
  deadline: Date;
  validity: Date;
  children: TuitionChildData[];
}
