export interface Session {
    id: number;
    scheduleId: number;
    teacherId: number;
    classId: number;
    courseId: number;
    startHourMin: Date;
    endHourMin: Date;
    sessionDate: Date;
    comment: string;
}
