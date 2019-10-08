export interface Schedule {
    id: number;
    classId: number;
    courseId: number;
    teacherId: number;
    day: number;
    startHourMin: Date;
    endHourMin: Date;
}
