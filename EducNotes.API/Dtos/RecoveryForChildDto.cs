using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
  public class RecoveryForChildDto
  {
    public int Id { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public string LevelName { get; set; }
    public string ClassName { get; set; }
    public string PhotoUrl { get; set; }
    public decimal LateDueAmount { get; set; }
    public List<ProductRecoveryDto> ProductRecovery { get; set; }

  }
}