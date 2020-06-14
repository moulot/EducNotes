using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
  public class RegistrationEmailDto
  {
    public int ParentId { get; set; }
    public string ParentFirstName { get; set; }
    public string ParentLastName { get; set; }
    public byte ParentGender { get; set; }
    public string ParentEmail { get; set; }
    public string EmailSubject { get; set; }
    public string ParentCellPhone { get; set; }
    public int OrderId { get; set; }
    public int OrderNum { get; set; }
    public string Token { get; set; }
    public List<ChildRegistrationDto> Children { get; set; }
    public string TotalAmount { get; set; }
    public string DueDate { get; set; }
  }
}