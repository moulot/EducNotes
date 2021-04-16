namespace EducNotes.API.Models
{
  public class ScheduleCourse : BaseClass
  {
    public int ScheduleId { get; set; }
    public Schedule Schedule { get; set; }
    public int? CourseId { get; set; }
    public Course Course { get; set; }
    public string ItemName { get; set; }
  }
}