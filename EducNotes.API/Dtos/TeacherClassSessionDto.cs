using System;

namespace EducNotes.API.Dtos
{
  public class TeacherClassSessionDto
  {
    public int ScheduleId { get; set; }
    public int TeacherId { get; set; }
    public string TeacherName { get; set; }
    public int CourseId { get; set; }
    public string CourseName { get; set; }
    public int ClassId { get; set; }
    public string ClassName { get; set; }
    public int Day { get; set; }
    public DateTime CourseStartHM { get; set; }
    public DateTime CourseEndHM { get; set; }
    public string StartHourMin { get; set; }
    public string EndHourMin { get; set; }
  }
}