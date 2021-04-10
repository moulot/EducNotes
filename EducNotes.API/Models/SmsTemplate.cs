using System;

namespace EducNotes.API.Models
{
  public class SmsTemplate
  {
    public SmsTemplate()
    {
      Internal = true;
    }
    
    public int Id { get; set; }
    public string Name { get; set; }
    public string Content { get; set; }
    public int SmsCategoryId { get; set; }
    public SmsCategory SmsCategory { get; set; }
    public Boolean Internal { get; set; }
  }
}