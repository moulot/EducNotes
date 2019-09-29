export interface Schedule {
    id: number;
    classId: number;
    courseId: number;
    day: number;
    startHourMin: Date;
    endHourMin: Date;
}
