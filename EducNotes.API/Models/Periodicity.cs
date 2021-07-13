namespace EducNotes.API.Models
{
  public class Periodicity
  {
    public Periodicity()
    {
      NbDays = 0;
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public byte NbDays { get; set; }
  }
}