using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace EducNotes.API.Dtos
{
  public class ChildrenForEditDto
  {
    public List<int> Id { get; set; }
    public List<string> LastName { get; set; }
    public List<string> FirstName { get; set; }
    public List<string> UserName { get; set; }
    public List<string> PhoneNumber { get; set; }
    public List<string> SecondPhoneNumber { get; set; }
    public List<int> UserTypeId { get; set; }
    public List<byte> Gender { get; set; }
    public List<string> Email { get; set; }
    public List<DateTime> DateOfBirth { get; set; }
    public List<string> strDateOfBirth { get; set; }
    public List<string> PhotoUrl { get; set; }
    public List<IFormFile> PhotoFiles { get; set; }
  }
}