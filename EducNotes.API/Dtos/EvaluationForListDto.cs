using System;

namespace EducNotes.API.Dtos
{
  public class EvaluationForListDto
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public int ClassId { get; set; }
    public string ClassName { get; set; }
    public int CourseId { get; set; }
    public string CourseName { get; set; }
    public string CourseAbbrev { get; set; }
    public string CourseColor { get; set; }
    public string EvalTypeName { get; set; }
    public string EvalDate { get; set; }
    public Boolean Graded { get; set; }
    public Boolean CanBeNegative { get; set; }
    public decimal Coeff { get; set; }
    public Boolean GradeInLetter { get; set; }
    public string MaxGrade { get; set; }
    public Boolean Significant { get; set; }
    public byte Closed { get; set; }
  }
}