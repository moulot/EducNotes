using System;
using EducNotes.API.Models;

namespace EducNotes.API.Dtos
{
  public class ScheduleCourseDto
  {
    public int Id { get; set; }
    public int ScheduleId { get; set; }
    public int CourseId { get; set; }
    public string CourseName { get; set; }
    public string CourseAbbrev { get; set; }
    public string CourseColor { get; set; }
    public int TeacherId { get; set; }
    public byte Gender { get; set; }
    public string TeacherName { get; set; }
    public string TeacherLastName { get; set; }
    public string TeacherFirstName { get; set; }
    public int ClassId { get; set; }
    public string ClassName { get; set; }
    public DateTime StartHour { get; set; }
    public string StartH { get; set; }
    public DateTime EndHour { get; set; }
    public string EndH { get; set; }
    public string DelInfo { get; set; }
    public Boolean InConflict { get; set; }
    public Boolean IsDarkColor { get; set; }
  }
}