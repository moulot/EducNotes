using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
  public class RoleDto
  {
    public int RoleId { get; set; }
    public string RoleName { get; set; }
    public List<RoleCapabilityDto> Capabilities { get; set; }
  }
}