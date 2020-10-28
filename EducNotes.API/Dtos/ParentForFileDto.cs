using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
  public class ParentForFileDto
  {
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string idNum { get; set; }
    public string Email { get; set; }
    public byte Gender { get; set; }
    public string strDateOfBirth { get; set; }
    public int Age { get; set; }
    public string PhotoUrl { get; set; }
    public string PhoneNumber { get; set; }
    public List<UserForDetailedDto> Children { get; set; }
    public List<EmailForListDto> EmailSent { get; set; }
    public List<SmsForListDto> SmsSent { get; set; }
  }
}