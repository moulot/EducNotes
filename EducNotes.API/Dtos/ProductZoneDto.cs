namespace EducNotes.API.Dtos
{
  public class ProductZoneDto
  {
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public int ZoneId { get; set; }
    public string ZoneName { get; set; }
    public decimal Price { get; set; }
  }
}