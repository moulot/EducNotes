using System.Collections.Generic;
using EducNotes.API.Models;

namespace EducNotes.API.Dtos
{
  public class ClassByLevelDto
  {
    public int ClassLevelId { get; set; }
    public string LevelName { get; set; }
    public int EducLevelId { get; set; }
    public List<Class>  Classes { get; set; }
  }
}