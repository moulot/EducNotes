using System;
using System.Collections.Generic;
using EducNotes.API.Models;

namespace EducNotes.API.Dtos
{
  public class UserScheduleNDaysDto
  {
    public int UserId { get; set; }
    public User User { get; set; }
    public DateTime DayDate { get; set; }
    public string strDayDate { get; set; }
    public int Day { get; set; }
    public List<UserDayEventsDto> Events { get; set; }
  }
}