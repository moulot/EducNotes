export interface TeacherForEdit {
  id: number;
  lastName: string;
  firstName: string;
  userName: string;
  phoneNumber: string;
  secondPhoneNumber: string;
  gender: number;
  email: string;
  dateOfBirth: Date;
  strDateOfBirth: string;
  photoUrl: string;
  photoFile: File;
  courseIds: string;
  active: number;
}
