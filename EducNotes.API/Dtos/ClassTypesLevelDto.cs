using System.Collections.Generic;
using EducNotes.API.Models;

namespace EducNotes.API.Dtos
{
  public class ClassTypesLevelDto
  {
    public int LevelId { get; set; }
    public string LevelName { get; set; }
    public int CycleId { get; set; }
    public List<ClassType> ClassTypes { get; set; }
  }
}