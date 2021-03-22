using System;

namespace EducNotes.API.Dtos
{
  public class ScheduleCourseDto
  {
    public int CourseId { get; set; }
    public string CourseName { get; set; }
    public string CourseAbbrev { get; set; }
    public string CourseColor { get; set; }
    public int ClassId { get; set; }
    public string ClassName { get; set; }
    public int TeacherId { get; set; }
    public DateTime StartHour { get; set; }
    public DateTime EndHour { get; set; }
    public string StartH { get; set; }
    public string EndH { get; set; }
    public Boolean InConflict { get; set; }
    public Boolean IsDarkColor { get; set; }
  }
}