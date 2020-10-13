using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
  public class NextCoursesByClassDto
  {
    public int TeacherId { get; set; }
    public string TeacherName { get; set; }
    public int ClassId { get; set; }
    public string ClassName { get; set; }
    public List<ClassNextCoursesDto> Courses { get; set; }
  }
}