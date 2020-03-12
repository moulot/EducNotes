using System;

namespace EducNotes.API.Models
{
    public class Session
    {
      public int Id { get; set; }
      public int? ScheduleId { get; set; }
      public Schedule Schedule { get; set; }
      public int TeacherId { get; set; }
      public User Teacher { get; set; }
      public int ClassId { get; set; }
      public Class Class { get; set; }
      public int CourseId { get; set; }
      public Course Course { get; set; }
      public DateTime StartHourMin { get; set; }
      public DateTime EndHourMin { get; set; }
      public DateTime SessionDate { get; set; }
      public string Comment { get; set; }
    }
}