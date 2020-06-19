using System;

namespace EducNotes.API.Dtos
{
  public class UserDataToUpdateDto
  {
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public string UserName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Pwd { get; set; }
    public string Email { get; set; }
    public string Cell { get; set; }
  }
}