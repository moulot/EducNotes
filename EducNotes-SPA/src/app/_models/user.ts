import { Photo } from './photo';

export interface User {
  id: number;
  idNum: string;
  userName: string;
  firstName: string;
  lastName: string;
  knownAs: string;
  age: number;
  created: Date;
  lastActive: Date;
  photoUrl: string;
  country: string;
  interests?: string;
  introduction?: string;
  lookingFor?: string;
  userTypeId: number;
  educLevelId?: number;
  classLevelId?: number;
  classLevelName: string;
  classId?: number;
  teacherClassId?: number;
  className: string;
  email: string;
  emailConfirmed: boolean;
  gender: number;
  dateOfBirth: Date;
  userTypeName: string;
  phoneNumber: string;
  phoneNumberConfirmed: boolean;
  secondPhoneNumber: string;
  districtId: number;
  cityId?: number;
  active: number;
  validated: boolean;
  accountDataValidated: boolean;

  photos?: Photo[];
  roles?: string[];
}
