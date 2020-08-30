using System.Collections.Generic;
using EducNotes.API.Models;

namespace EducNotes.API.Dtos
{
  public class RolesWithUsersDto
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public List<UserForDetailedDto> RoleUsers { get; set; }
  }
}