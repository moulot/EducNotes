export interface ScheduleData {
  id: number;
  classId: number;
  courseId: number;
  conflictedCourseId: number;
  scheduleId: number;
  teacherId: number;
  day: number;
  startHour: number;
  startMin: number;
  endHour: number;
  endMin: number;
}
