using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
  public class AbsenceClassDto
  {
    public int ClassId { get; set; }
    public string ClassName { get; set; }
    public int TotalAbs { get; set; }
    public int TotalLates { get; set; }
    public List<AbsenceChildDto> Children { get; set; }
  }
}