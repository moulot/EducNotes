using System.Collections.Generic;
using EducNotes.API.Models;

namespace EducNotes.API.Dtos
{
  public class UserToValidateDto
  {
    public int Id { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public string ClassName { get; set; }
    public string ClassLevelName { get; set; }
    public string UserType { get; set; }
    public string Email { get; set; }
    public string Cell { get; set; }
    public int MotherId { get; set; }
    public string MotherFirstName { get; set; }
    public string MotherLastName { get; set; }
    public string MotherEmail { get; set; }
    public string MotherCell { get; set; }
    public int FatherId { get; set; }
    public string FatherFirstName { get; set; }
    public string FatherLastName { get; set; }
    public string FatherEmail { get; set; }
    public string FatherCell { get; set; }
    public List<ChildParentDto> Children { get; set; }
    public OrderUserToValidateDto Tuition { get; set; }
  }
}