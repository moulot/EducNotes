import { ClassLevel } from './classLevel';
import { Theme } from './theme';
import { Course } from './course';
import { LessonContent } from './lesson-content';

// import {} from 'class-validator';

export class Lesson {
  public id: number;

  public classLevelId: number;

  public classLevel: ClassLevel;

  public courseId: number;

  public course: Course;

  public themeId: number;

  public theme: Theme;

  public name: string;

  public desc: string;
  public lessonContents: LessonContent[];

  constructor(
    id?: number,
    classLevelId?: number,
    classLevel?: ClassLevel,
    courseId?: number,
    course?: Course,
    themeId?: number,
    theme?: Theme,
    name?: string,
    desc?: string,
  ) {
    this.id = id || 0;
    this.classLevelId = classLevelId || null;
    this.courseId = courseId || null;
    this.themeId = themeId || null;
    this.name = name || '';
    this.desc = desc || '';
  }
}
