import { Photo } from './photo';

export interface User {
    id: number;
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
    classId?: number;
    className: string;
    email: string;
    gender: number;
    dateOfBirth: Date;
    userTypeName: string;
    emailConfirmed: boolean;
    phoneNumber: string;
    secondPhoneNumber: string;
    validatedCode: boolean;
    districtId: number;
    cityId?: number;
    active: number;

    photos?: Photo[];
    roles?: string[];
}
