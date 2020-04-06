using System.Collections.Generic;
using EducNotes.API.Models;

namespace EducNotes.API.Dtos
{
  public class AssignedClassesDto
  {
    public int CourseId { get; set; }
    public string CourseName { get; set; }
    public List<LevelWithClassesDto> Levels { get; set; }
  }
}