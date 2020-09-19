using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
  public class ClasslevelProductDto
  {
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public int ClassLevelId { get; set; }
    public string LevelName { get; set; }
    public byte DsplSeq { get; set; }
    public decimal Price { get; set; }
  }
}