using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
  public class RecoveryForLevelDto
  {
    public int LevelId { get; set; }
    public string LevelName { get; set; }
    public decimal LateAmountDue { get; set; }
    public List<ProductRecoveryDto> ProductRecovery { get; set; }
  }
}