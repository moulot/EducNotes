namespace EducNotes.API.Models
{
  public class LocationZone
  {
    public int Id { get; set; }
    public int ZoneId { get; set; }
    public Zone Zone { get; set; }
    public int? DistrictId { get; set; }
    public District District { get; set; }
    public int? CityId { get; set; }
    public City City { get; set; }
    public int? CountryId { get; set; }
    public Country Country { get; set; }
  }
}