// import {} from 'class-validator';
import { Lesson } from './lesson';

export class LessonContent {
  public id: number;

  public name: string;

  public desc: string;

  public lessonId: number;

  public lesson: Lesson;

  public nbHours: number;

  public sessionNum: number;

  constructor(
    id?: number,
    name?: string,
    desc?: string,
    lessonId?: number,
    lesson?: Lesson,
    nbHours?: number,
    sessionNum?: number,
    ) {
    this.id = id || 0;
    this.name = name || '';
    this.desc = desc || '';
    this.lessonId = lessonId || 0;
    this.nbHours = nbHours || 0;
    this.sessionNum = sessionNum || null;
  }
}
