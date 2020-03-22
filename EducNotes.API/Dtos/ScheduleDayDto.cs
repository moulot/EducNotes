using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
  public class ScheduleDayDto
  {
    public int Day { get; set; }
    public string DayName { get; set; }
    public List<ScheduleCourseDto> Courses { get; set; }
  }
}