using System.Collections.Generic;

namespace EducNotes.API.Models
{
  public class Course
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Abbreviation { get; set; }
    public string Color { get; set; }
    public ICollection<CourseSkill> CourseSkills { get; set; }
  }
}