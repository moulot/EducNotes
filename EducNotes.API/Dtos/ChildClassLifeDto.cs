using System.Collections.Generic;
using EducNotes.API.Models;

namespace EducNotes.API.Dtos
{
  public class ChildClassLifeDto
  {
    public int Id { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public int TotalAbsences { get; set; }
    public int TotalLates { get; set; }
    public int TotalSanctions { get; set; }
    public int TotalRewards { get; set; }
    public List<AbsenceChildDetailsDto> Absences { get; set; }
    public List<SanctionDataDto> Sanctions { get; set; }
    public List<RewardDataDto> Rewards { get; set; }
  }
}