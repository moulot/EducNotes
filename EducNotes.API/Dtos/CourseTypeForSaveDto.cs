using System;

namespace EducNotes.API.Dtos
{
  public class CourseTypeForSaveDto
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public Boolean ToBeDeleted { get; set; }
  }
}