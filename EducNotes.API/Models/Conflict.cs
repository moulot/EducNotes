using System;

namespace EducNotes.API.Models
{
  public class Conflict : BaseEntity
  {
    public int ScheduleId { get; set; }
    public Schedule Schedule { get; set; }
    public int ClassId { get; set; }
    public Class Class { get; set; }
    public int Day { get; set; }
  }
}