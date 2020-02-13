using System.Collections.Generic;

namespace EducNotes.API.Models
{
  public class Theme
  {
    public int Id { get; set; }
    public int ClassLevelId { get; set; }
    public ClassLevel ClassLevel { get; set; }
    public int CourseId { get; set; }
    public Course Course { get; set; }
    public string Name { get; set; }
    public string Desc { get; set; }
    public ICollection<Lesson> Lessons { get; set; }
  }
}