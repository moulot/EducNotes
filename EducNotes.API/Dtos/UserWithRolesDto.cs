using System;
using System.Collections.Generic;
using EducNotes.API.Models;

namespace EducNotes.API.Dtos
{
  public class UserWithRolesDto
  {
    public int Id { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public string PhotoUrl { get; set; }
    public Boolean Validated { get; set; }
    public List<Role> Roles { get; set; }
  }
}