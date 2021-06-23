namespace EducNotes.API.Models
{
  public class ProductZone
  {
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; }
    public int ZoneId { get; set; }
    public Zone Zone { get; set; }
    public decimal Price { get; set; }
  }
}