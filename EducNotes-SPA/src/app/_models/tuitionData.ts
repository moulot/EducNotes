import { TuitionChildData } from './tuitionChildData';

export interface TuitionData {
  fLastName: string;
  fFirstName: string;
  fCell: string;
  fEmail: string;
  // fSendEmail: boolean;
  fActive: boolean;
  mLastName: string;
  mFirstName: string;
  mCell: string;
  mEmail: string;
  // mSendEmail: boolean;
  mActive: boolean;
  orderAmount: number;
  dueAmount: number;
  deadline: Date;
  validity: Date;
  children: TuitionChildData[];
}
