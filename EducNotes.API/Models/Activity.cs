namespace EducNotes.API.Models
{
    public class Activity : BaseEntity
    {
      public string Name { get; set; }
      public string Abbreviation { get; set; }
      public string Color { get; set; }
    }
}