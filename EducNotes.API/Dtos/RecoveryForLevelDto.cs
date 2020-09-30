using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
  public class RecoveryForLevelDto
  {
    public string LevelName { get; set; }
    public decimal LateAmount { get; set; }
    public List<ProductRecoveryDto> ProductRecovery { get; set; }
  }
}