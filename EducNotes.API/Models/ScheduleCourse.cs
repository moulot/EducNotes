namespace EducNotes.API.Models
{
  public class ScheduleCourse : BaseEntity
  {
    public int ScheduleId { get; set; }
    public Schedule Schedule { get; set; }
    public int TeacherId { get; set; }
    public User Teacher { get; set; }
    public int CourseId { get; set; }
    public Course Course { get; set; }
  }
}