using System;
using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
  public class ScheduleForTimeTableDto
  {
    public int Id { get; set; }
    public int ClassId { get; set; }
    public string ClassName { get; set; }
    public string ClassLevel { get; set; }
    public int? TeacherId { get; set; }
    public string TeacherName { get; set; }
    public int Day { get; set; }
    public DateTime StartHourMin { get; set; }
    public string strStartHourMin { get; set; }
    public string strEndHourMin { get; set; }
    public string Top { get; set; }
    public string Height { get; set; }
    public string Color { get; set; }
    public Boolean IsDarkColor { get; set; }
    public string DelInfo { get; set; }
    public List<ScheduleCourseDto> Courses { get; set; }
  }
}