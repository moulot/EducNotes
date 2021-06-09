
namespace EducNotes.API.Dtos
{
  public class ClassDetailDto
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public int ClassTypeId { get; set; }
    public int ClassLevelId { get; set; }
    public string LevelName { get; set; }
    public int EducationLevelId { get; set; }
    public int SchoolId { get; set; }
    public int CycleId { get; set; }
    public int MaxStudent { get; set; }
    public int TotalStudents { get; set; }
  }
}