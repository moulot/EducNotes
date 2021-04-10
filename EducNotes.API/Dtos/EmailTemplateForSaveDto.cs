using System;

namespace EducNotes.API.Dtos
{
  public class EmailTemplateForSaveDto
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public int EmailCategoryId { get; set; }
    public Boolean Internal { get; set; }
  }
}