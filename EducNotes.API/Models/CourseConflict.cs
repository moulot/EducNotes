namespace EducNotes.API.Models
{
  public class CourseConflict : BaseEntity
  {
    public int ConflictId { get; set; }
    public Conflict Conflict { get; set; }
    public int ScheduleCourseId { get; set; }
    public ScheduleCourse ScheduleCourse { get; set; }
  }
}