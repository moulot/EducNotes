namespace EducNotes.API.Models
{
  public class SmsTemplate
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Content { get; set; }
    public int SmsCategoryId { get; set; }
    public SmsCategory SmsCategory { get; set; }
  }
}