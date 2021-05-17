using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
  public class RoleDto
  {
    public RoleDto()
    {
      Capabilities = new List<RoleCapabilityDto>();
      UsersInRole = new List<UserInRoleDto>();
    }

    public int RoleId { get; set; }
    public string RoleName { get; set; }
    public List<RoleCapabilityDto> Capabilities { get; set; }
    public List<UserInRoleDto> UsersInRole { get; set; }
  }
}