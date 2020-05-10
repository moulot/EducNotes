using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
  public class RegistrationEmailDto
  {
    public string ParentFirstName { get; set; }
    public string ParentLastName { get; set; }
    public byte ParentGender { get; set; }
    public string ParentEmail { get; set; }
    public string EmailSubject { get; set; }
    public string ParentCellPhone { get; set; }
    public List<ChildRegistrationDto> Children { get; set; }
    public string TotalAmount { get; set; }
    public string DueDate { get; set; }
  }
}