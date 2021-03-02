using System;
using Microsoft.AspNetCore.Http;

namespace EducNotes.API.Dtos
{
  public class UserAccountForEditDto
  {
    public int Id { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public Boolean PwdChanged { get; set; }
    public string PhoneNumber { get; set; }
    public string SecondPhoneNumber { get; set; }
    public string OldEmail { get; set; }
    public string OldPhoneNumber { get; set; }
    public byte Gender { get; set; }
    public string Email { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string strDateOfBirth { get; set; }
    public int CityId { get; set; }
    public int DistrictId { get; set; }
    public DateTime LastActive { get; set; }
    public string PhotoUrl { get; set; }
    public IFormFile PhotoFile { get; set; }
  }
}