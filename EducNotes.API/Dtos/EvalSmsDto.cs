using System;

namespace EducNotes.API.Dtos
{
  public class EvalSmsDto
  {
    public int EvaluationId { get; set; }
    public Boolean ForUpdate { get; set; }
    public int ChildId { get; set; }
    public string ChildFirstName { get; set; }
    public string ChildLastName { get; set; }
    public int ParentId { get; set; }
    public string ParentFirstName { get; set; }
    public string ParentLastName { get; set; }
    public byte ParentGender { get; set; }
    public string EvalDate { get; set; }
    public string CapturedDate { get; set; }
    public string CourseName { get; set; }
    public string CourseAbbrev { get; set; }
    public double OldEvalGrade { get; set; }
    public double EvalGrade { get; set; }
    public string GradedOutOf { get; set; }
    public double ClassMinGrade { get; set; }
    public double ClassMaxGrade { get; set; }
    public double ChildCourseAvg { get; set; }
    public double ChildAvg { get; set; }
    public double ClassCourseAvg { get; set; }
    public double ClassAvg { get; set; }
    public string ParentCellPhone { get; set; }

  }
}