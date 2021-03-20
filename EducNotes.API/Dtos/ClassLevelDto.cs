using System.Collections.Generic;
using EducNotes.API.Models;

namespace EducNotes.API.Dtos
{
  public class ClassLevelDto
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public int? SchoolId { get; set; }
    public int? CycleId { get; set; }
    public int? EducationLevelId { get; set; }
    public byte DsplSeq { get; set; }
    public ICollection<Inscription> Inscriptions { get; set; }
    public ICollection<Class> Classes { get; set; }
  }
}