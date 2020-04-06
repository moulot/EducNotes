using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
  public class LevelWithClassesDto
  {
    public int LevelId { get; set; }
    public string LevelName { get; set; }
    public List<ClassIdAndNameDto> Classes { get; set; }
  }
}