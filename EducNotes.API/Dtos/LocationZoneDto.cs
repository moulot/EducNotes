namespace EducNotes.API.Dtos
{
  public class LocationZoneDto
  {
    public int ZoneId { get; set; }
    public string ZoneName { get; set; }
    public int DistrictId { get; set; }
    public string DistrictName { get; set; }
    public int CityId { get; set; }
    public string CityName { get; set; }
    public int CountryId { get; set; }
    public string CountryName { get; set; }
  }
}