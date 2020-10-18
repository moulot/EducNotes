namespace EducNotes.API.Models
{
  public class Client
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string SubDomain { get; set; }
    public string ConnectionString { get; set; }
  }
}