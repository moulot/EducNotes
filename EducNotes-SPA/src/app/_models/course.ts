import { CourseSkill } from './courseSkill';

export class Course {
    id: number;
    name: string;
    abbreviation: string;
    color: string;
    courseSkills: CourseSkill[];
}
