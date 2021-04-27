using EducNotes.API.Models;

namespace EducNotes.API.Dtos
{
  public class ScheduleDataDto
  {
    public int Id { get; set; }
    public int ClassId { get; set; }
    public int CourseId { get; set; }
    public int ConflictedCourseId { get; set; }
    public int ScheduleId { get; set; }
    public int TeacherId { get; set; }
    public int ActivityId { get; set; }
    public int Day { get; set; }
    public int StartHour { get; set; }
    public int StartMin { get; set; }
    public int EndHour { get; set; }
    public int EndMin { get; set; }
  }
}