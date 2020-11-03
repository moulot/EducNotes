using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
  public class AbsenceDataDto
  {
    public int TotalAbs { get; set; }
    public int TotalLates { get; set; }
    public List<AbsenceChildDetailsDto> Absences { get; set; }
  }
}