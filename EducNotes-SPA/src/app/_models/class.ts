import { User } from './user';

export interface Class {
    id: number;
    classLevelId: number;
    mainTeacherId: number;
    name: string;
    active: number;
    maxStudent: number;
    totalStudents: number;
    students: User[];
}
