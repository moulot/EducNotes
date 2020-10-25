using System;
using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
  public class AbsenceChildDto
  {
    public int Id { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public string PhotoUrl { get; set; }
    public int TotalAbsences { get; set; }
    public int TotalLates { get; set; }
    public List<AbsenceChildDetailsDto> Absences { get; set; }
  }
}