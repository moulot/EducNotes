namespace EducNotes.API.Dtos
{
  public class BroadcastMessageDto
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Subject { get; set; }
    public string BodyContent { get; set; }
    public string EmailCategoryName { get; set; }
    public string SmsCategoryName { get; set; }
  }
}