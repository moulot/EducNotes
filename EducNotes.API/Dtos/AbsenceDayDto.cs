using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
  public class AbsenceDayDto
  {
    public int Day { get; set; }
    public string strDay { get; set; }
    public string strDayShort { get; set; }
    public string strDayLong { get; set; }
    public List<AbsenceClassDto> Classes { get; set; }
  }
}