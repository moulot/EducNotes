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
    public int EventTypeId { get; set; }
    public EventType EventType { get; set; }
    public string StartHourMin { get; set; }
    public string EndHourMin { get; set; }
    
  }
}