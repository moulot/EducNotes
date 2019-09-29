namespace EducNotes.API.Dtos
{
  public class PeriodToReturnDto
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string StartDate { get; set; }
    public string EndDate { get; set; }
    public byte Active { get; set; }
  }
}