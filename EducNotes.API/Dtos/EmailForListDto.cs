namespace EducNotes.API.Dtos
{
  public class EmailForListDto
  {
    public int EmailTypeId { get; set; }
    public string EmailType { get; set; }
    public string ToAddress { get; set; }
    public string CCAddress { get; set; }
    public string BCCAddress { get; set; }
    public string FromAddress { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public string DateSent { get; set; }
    public string SentBy { get; set; }
  }
}