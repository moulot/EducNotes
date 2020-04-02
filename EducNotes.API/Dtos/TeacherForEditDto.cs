using System;
using Microsoft.AspNetCore.Http;

namespace EducNotes.API.Dtos
{
  public class TeacherForEditDto
  {
    public int Id { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public string UserName { get; set; }
    public string PhoneNumber { get; set; }
    public string SecondPhoneNumber { get; set; }
    public int UserTypeId { get; set; }
    public byte Gender { get; set; }
    public string Email { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string strDateOfBirth { get; set; }
    public DateTime LastActive { get; set; }
    public string PhotoUrl { get; set; }
    public IFormFile PhotoFile { get; set; }
    public string CourseIds { get; set; }
  }
}