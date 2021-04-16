using System;

namespace EducNotes.API.Models
{
  public class Schedule : BaseEntity
  {
    public int ClassId { get; set; }
    public Class Class { get; set; }
    public int Day { get; set; }
    public DateTime StartHourMin { get; set; }
    public DateTime EndHourMin { get; set; }
  }
}