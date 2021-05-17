using System.Collections.Generic;
using EducNotes.API.Models;

namespace EducNotes.API.Dtos
{
  public class RoleWithUsersDto
  {
    public RoleWithUsersDto()
    {
      Capabilities = new List<RoleCapability>();
      Users = new List<UserInRoleDto>();
    }
    
    public int Id { get; set; }
    public string Name { get; set; }
    public List<RoleCapability> Capabilities { get; set; }
    public List<UserInRoleDto> Users { get; set; }
  }
}