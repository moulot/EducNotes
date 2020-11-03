using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
  public class UserCourseDto
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Abbrev { get; set; }
    public double Coeff { get; set; }
    public double GradedOutOf { get; set; }
    public double UserAvg { get; set; }
    public bool UserAvgOK { get; set; }
    public double ClassAvg { get; set; }
    public double ClassAvgMin { get; set; }
    public double ClassAvgMax { get; set; }
    public List<GradeDto> Grades { get; set; }
    public List<PeriodEvalsDto> PeriodEvals { get; set; }
  }
}