using System;

namespace EducNotes.API.Dtos
{
  public class WeeklyAbsenceDto
  {
    public DateTime StartDate { get; set; }
    public int MoveDays { get; set; }
  }
}