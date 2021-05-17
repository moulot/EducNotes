namespace EducNotes.API.Dtos
{
  public class RoleCapabilityDto
  {
    public int RoleId { get; set; }
    public int CapabilityId { get; set; }
    public byte AccessFlag { get; set; }
  }
}