using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
  public class ClassLevelCourseDto
  {
    public int LevelId { get; set; }
    public List<CLCourseDto> Courses { get; set; }
  }
}