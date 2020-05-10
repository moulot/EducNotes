using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
  public class EducationLevelDto
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public List<ClassDetailDto> Classes { get; set; }
  }
}