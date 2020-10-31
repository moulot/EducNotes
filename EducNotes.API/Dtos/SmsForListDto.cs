namespace EducNotes.API.Dtos
{
  public class SmsForListDto
  {
    public string SmsType { get; set; }
    public string To { get; set; }
    public string From { get; set; }
    public string Content { get; set; }
    public string DateSent { get; set; }
    public string SentBy { get; set; }
  }
}