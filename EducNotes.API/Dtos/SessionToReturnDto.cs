using System;

namespace EducNotes.API.Dtos
{
  public class SessionToReturnDto
  {
    public int Id { get; set; }
    public int ScheduleId { get; set; }
    public int TeacherId { get; set; }
    public string TeacherName { get; set; }
    public int CourseId { get; set; }
    public string CourseName { get; set; }
    public string CourseAbbrev { get; set; }
    public string CourseColor { get; set; }
    public int ClassId { get; set; }
    public string ClassName { get; set; }
    public DateTime SessionDate { get; set; }
    public string strSessionDate { get; set; }
    public string StartHourMin { get; set; }
    public string EndHourMin { get; set; }
    public string Comment { get; set; }
  }
}