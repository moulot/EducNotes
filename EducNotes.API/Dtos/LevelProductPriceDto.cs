namespace EducNotes.API.Dtos
{
  public class LevelProductPriceDto
  {
    public int Id { get; set; }
    public int LevelId { get; set; }
    public string LevelName { get; set; }
    public decimal Price { get; set; }
  }
}