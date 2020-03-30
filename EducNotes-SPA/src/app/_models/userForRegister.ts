export interface UserForRegister {
  id: number;
  userName: string;
  firstName: string;
  lastName: string;
  photoUrl: string;
  country: string;
  userTypeId: number;
  classId?: number;
  email: string;
  gender: number;
  dateOfBirth: Date;
  emailConfirmed: boolean;
  phoneNumber: string;
  secondPhoneNumber: string;
  validatedCode: boolean;
  districtId: number;
  cityId?: number;
  active: number;
  courseIds: string;
  photoFile: File;
}
