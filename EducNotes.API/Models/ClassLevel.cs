using System.Collections.Generic;

namespace EducNotes.API.Models
{
  public class ClassLevel
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string NameAbbrev { get; set; }
    public int? SchoolId { get; set; }
    public School School { get; set; }
    public int? CycleId { get; set; }
    public Cycle Cycle { get; set; }
    public int? EducationLevelId { get; set; }
    public EducationLevel EducationLevel { get; set; }
    public byte DsplSeq { get; set; }
    public ICollection<Inscription> Inscriptions { get; set; }
  }
}