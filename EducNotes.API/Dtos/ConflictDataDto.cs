using System;

namespace EducNotes.API.Dtos
{
  public class ConflictDataDto
  {
    public int ScheduleId { get; set; }
    public int ConflictedCourseId { get; set; }
    public int ClassId { get; set; }
    public int Day { get; set; }
    public DateTime StartHourMin { get; set; }
    public DateTime EndHourMin { get; set; }
    public int CourseId { get; set; }
    public int TeacherId { get; set; }
  }
}