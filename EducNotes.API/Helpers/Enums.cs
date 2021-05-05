namespace EducNotes.API.Helpers
{
  public class Enums
  {
    public enum AccessType
    {
      ReadOnlyEdit = 0,
      ReadOnly = 1,
      Edit = 2
    }

    public enum CapabilityAccessFlag
    {
      None,
      ReadOnly,
      Edit
    }
  }
}