using System;
using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
  public class UserCourseEvalsDto
  {
    public int StudentId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int CourseId { get; set; }
    public string CourseName { get; set; }
    public string CourseAbbrev { get; set; }
    public double CourseCoeff { get; set; }
    public double GradedOutOf { get; set; }
    public double UserCourseAvg { get; set; }
    public double ClassCourseAvg { get; set; }
    public List<GradeDto> Grades { get; set; }
    public List<PeriodEvalsDto> PeriodEvals { get; set; }
  }
}