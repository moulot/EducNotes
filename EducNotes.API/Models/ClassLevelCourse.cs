namespace EducNotes.API.Models
{
  public class ClassLevelCourse
  {
    public int Id { get; set; }
    public int ClassLevelId { get; set; }
    public ClassLevel ClassLevel { get; set; }
    public int CourseId { get; set; }
    public Course Course { get; set; }
  }
}