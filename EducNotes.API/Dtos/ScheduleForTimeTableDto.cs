using System;
using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
  public class ScheduleForTimeTableDto
  {
    public int Id { get; set; }
    public int ClassId { get; set; }
    public string ClassName { get; set; }
    public string ClassLevelName { get; set; }
    public int Day { get; set; }
    public DateTime StartHourMin { get; set; }
    public string strStartHourMin { get; set; }
    public DateTime EndHourMin { get; set; }
    public string strEndHourMin { get; set; }
    public string Top { get; set; }
    public string Height { get; set; }
    public List<ScheduleCourseDto> Courses { get; set; }
  }
}