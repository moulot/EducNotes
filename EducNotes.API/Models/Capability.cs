namespace EducNotes.API.Models
{
  public class Capability : BaseEntity
  {
    public string Name { get; set; }
    public int MenuItemId { get; set; }
    public MenuItem MenuItem { get; set; }
    public byte AccessType { get; set; }
  }
}