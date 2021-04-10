using System;

namespace EducNotes.API.Models
{
  public class EmailTemplate
  {
    public EmailTemplate()
    {
      Internal = true;
    }
    
    public int Id { get; set; }
    public string Name { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public int EmailCategoryId { get; set; }
    public EmailCategory EmailCategory { get; set; }
    public Boolean Internal { get; set; }
  }
}