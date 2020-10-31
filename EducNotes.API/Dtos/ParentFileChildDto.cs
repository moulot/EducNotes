namespace EducNotes.API.Dtos
{
  public class ParentFileChildDto
  {
    public UserForDetailedDto Child { get; set; }
    public int OrderId { get; set; }
    public int OrderLineId { get; set; }

  }
}