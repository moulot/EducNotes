namespace EducNotes.API.Models
{
  public class ClassLevelProduct
  {
    public int Id { get; set; }
    public decimal Price { get; set; }
    public int ClassLevelId { get; set; }
    public ClassLevel ClassLevel { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; }

  }
}