using System;
using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
  public class RecoveryDto
  {
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<ClassLevelRecoveryDto> LevelRecovery { get; set; }
    public List<ChildRecoveryDto> ChildRecovery { get; set; }
  }
}