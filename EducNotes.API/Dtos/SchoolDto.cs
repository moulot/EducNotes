using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
  public class SchoolDto
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public List<ClassDetailDto> Classes { get; set; }
  }
}