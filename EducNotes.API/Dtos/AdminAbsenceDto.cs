using System;
using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
  public class AdminAbsenceDto
  {
    public DateTime StartDate { get; set; }
    public string strStartDate { get; set; }
    public string EndDate { get; set; }
    public string LngStartDate { get; set; }
    public string LngEndDate { get; set; }
    public List<AbsenceDayDto> Days { get; set; }
  }
}