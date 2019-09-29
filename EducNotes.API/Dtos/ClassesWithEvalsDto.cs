using System.Collections.Generic;
using EducNotes.API.Models;

namespace EducNotes.API.Dtos
{
  public class ClassesWithEvalsDto
  {
    public int ClassId { get; set; }
    public string ClassName { get; set; }
    public string MainTeacher { get; set; }
    public int NbStudents { get; set; }
    public int NbEvals { get; set; }
    public List<EvaluationForListDto> OpenedEvals { get; set; }
    public List<EvaluationForListDto> ToBeGradedEvals { get; set; }
  }
}