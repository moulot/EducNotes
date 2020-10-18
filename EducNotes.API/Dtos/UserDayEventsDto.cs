using System;
using EducNotes.API.Models;

namespace EducNotes.API.Dtos
{
  public class UserDayEventsDto
  {
    public DateTime EventDate { get; set; }
    public string strEventDate { get; set; }
    public string Title { get; set; }
    public string Desc { get; set; }
    public string EventTypeName { get; set; }
    public string ClassName { get; set; }
    public int StartHour { get; set; }
    public int StartMin { get; set; }
    public string strStartHourMin { get; set; }
    public string strEndHourMin { get; set; }
    
  }
}