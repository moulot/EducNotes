namespace EducNotes.API.Dtos
{
    public class EmailTemplateForListDto
    {
      public int Id { get; set; }
      public string Name { get; set; }
      public string Subject { get; set; }
      public string Body { get; set; }
      public string EmailCategoryName { get; set; }
    }
}