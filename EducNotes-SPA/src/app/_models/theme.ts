import { ClassLevel } from './classLevel';
import { Course } from './course';
import { Lesson } from './lesson';

// import {} from 'class-validator';

export class Theme {
  public id: number;

  public classLevelId: number;

  public classLevel: ClassLevel;

  public courseId: number;

  public course: Course;

  public name: string;

  public desc: string;
  public lessons: Lesson[];

  constructor(
    id?: number,
    classLevelId?: number,
    classLevel?: ClassLevel,
    courseId?: number,
    course?: Course,
    name?: string,
    desc?: string,
  ) {
    this.id = id || 0;
    this.classLevelId = classLevelId || 0;
    this.courseId = courseId || 0;
    this.name = name || '';
    this.desc = desc || '';
  }
}
