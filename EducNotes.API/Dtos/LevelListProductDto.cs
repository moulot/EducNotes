using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
  public class LevelListProductDto
  {
    public int ProductId { get; set; }
    public List<int> ClassLevelIds { get; set; }
    public decimal Amount { get; set; }
  }
}