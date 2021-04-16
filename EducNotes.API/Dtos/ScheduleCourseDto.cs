using System;
using EducNotes.API.Models;

namespace EducNotes.API.Dtos
{
  public class ScheduleCourseDto
  {
    public Course Course { get; set; }
    public string ItemName { get; set; }
    public int ClassId { get; set; }
    public string ClassName { get; set; }
    public DateTime StartHour { get; set; }
    public string StartH { get; set; }
    public DateTime EndHour { get; set; }
    public string EndH { get; set; }
    public Boolean InConflict { get; set; }
    public Boolean IsDarkColor { get; set; }
  }
}