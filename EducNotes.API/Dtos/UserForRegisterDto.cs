using System;
using Microsoft.AspNetCore.Http;

namespace EducNotes.API.Dtos
{
  public class UserForRegisterDto
  {
    public UserForRegisterDto()
    {
      Created = DateTime.Now;
      LastActive = DateTime.Now;
    }

    public string LastName { get; set; }
    public string UserName { get; set; }
    public string FirstName { get; set; }
    public string PhoneNumber { get; set; }
    public string Password { get; set; }
    public string SecondPhoneNumber { get; set; }
    public int? ClassId { get; set; }
    public int? LevelId { get; set; }
    public int? DistrictId { get; set; }
    public int? CityId { get; set; }
    public int UserTypeId { get; set; }
    public byte? Gender { get; set; }
    public string Email { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public DateTime Created { get; set; }
    public DateTime LastActive { get; set; }
    public IFormFile PhotoFile { get; set; }
    public string CourseIds { get; set; }
  }
}