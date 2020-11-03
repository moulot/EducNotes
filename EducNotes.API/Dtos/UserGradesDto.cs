using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
  public class UserGradesDto
  {
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public double GradedOutOf { get; set; }
    public double Avg { get; set; }
    public bool AvgOK { get; set; }
    public double ClassAvg { get; set; }
    public double ClassAvgMin { get; set; }
    public double ClassAvgMax { get; set; }
    public List<GradeDto> LastGrades { get; set; }
    public List<UserCourseDto> Courses { get; set; }
  }
}