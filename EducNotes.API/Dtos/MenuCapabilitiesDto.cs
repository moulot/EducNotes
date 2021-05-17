using System.Collections.Generic;
using EducNotes.API.Models;

namespace EducNotes.API.Dtos
{
  public class MenuCapabilitiesDto
  {
    public MenuCapabilitiesDto()
    {
      Capabilities = new List<Capability>();
      ChildMenuItems = new List<MenuCapabilitiesDto>();
    }

    public int MenuItemId { get; set; }
    public string MenuItemName { get; set; }
    public List<Capability> Capabilities { get; set; }
    public List<MenuCapabilitiesDto> ChildMenuItems { get; set; }
  }
}