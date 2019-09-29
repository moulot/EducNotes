import { EvalProgElt } from './evalProgElt';

export interface Evaluation {
    id: number;
    userId: number;
    name: string;
    courseId: number;
    evalTypeId: number;
    evalDate: Date;
    graded: boolean;
    periodId: number;
    canBeNagative: boolean;
    classId: number;
    coeff: number;
    gradeInLetter: boolean;
    maxGrade: number;
    minGrade: number;
    significant: boolean;
    closed: number;
    progElts: EvalProgElt[];
}
