namespace EducNotes.API.Models
{
  public class ClassLevelClassType
  {
    public int Id { get; set; }
    public int ClassLevelId { get; set; }
    public ClassLevel ClassLevel { get; set; }
    public int ClassTypeId { get; set; }
    public ClassType ClassType { get; set; }
  }
}