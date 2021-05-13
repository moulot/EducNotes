namespace EducNotes.API.Models
{
  public class Menu
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public int UserTypeId { get; set; }
    public UserType UserType { get; set; }
  }
}