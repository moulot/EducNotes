export interface Agenda {
    id: number;
    sessionId: number;
    classId: number;
    courseId: number;
    dateAdded: Date;
    taskDesc: string;
    done: boolean;
    doneSetById: number;
}
