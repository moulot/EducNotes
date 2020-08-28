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
  classLevelId?: number;
  classId?: number;
  className: string;
  email: string;
  emailConfirmed: boolean;
  gender: number;
  dateOfBirth: Date;
  userTypeName: string;
  phoneNumber: string;
  phoneNumberConfirmed: boolean;
  secondPhoneNumber: string;
  // validatedCode: boolean;
  districtId: number;
  cityId?: number;
  active: number;
  validated: boolean;

  photos?: Photo[];
  roles?: string[];
}
