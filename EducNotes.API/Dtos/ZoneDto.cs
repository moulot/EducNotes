using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
  public class ZoneDto
  {
    public ZoneDto()
    {
      Locations = new List<LocationZoneDto>();
    }
    
    public int Id { get; set; }
    public string Name { get; set; }
    public List<LocationZoneDto> Locations { get; set; }
  }
}