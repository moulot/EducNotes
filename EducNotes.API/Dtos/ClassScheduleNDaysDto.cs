using System;
using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
  public class ClassScheduleNDaysDto
  {
    public int Day { get; set; }
    public DateTime DayDate { get; set; }
    public string strDayDate { get; set; }
    public List<ClassDayCoursesDto> Courses { get; set; }
  }
}