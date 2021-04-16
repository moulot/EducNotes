namespace EducNotes.API.Models
{
  public class RoleCapability : BaseEntity
  {
    public int RoleId { get; set; }
    public Role Role { get; set; }
    public int CapabilityId { get; set; }
    public Capability Capability { get; set; }
    public byte AccessFlag { get; set; }
  }
}