namespace EducNotes.API.Dtos
{
  public class GradeDto
  {
    public string CourseName { get; set; }
    public string CourseAbbrev { get; set; }
    public string EvalDate { get; set; }
    public string EvalType { get; set; }
    public string EvalTypeAbbrev { get; set; }
    public string EvalName { get; set; }
    public double Grade { get; set; }
    public double GradeMax { get; set; }
    public bool GradeOK { get; set; }
    public double Coeff { get; set; }
    public double ClassGradeMin { get; set; }
    public double ClassGradeMax { get; set; }
  }
}