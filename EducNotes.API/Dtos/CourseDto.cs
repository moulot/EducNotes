using System;
using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
  public class CourseDto
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public int CourseTypeId { get; set; }
    public string Abbrev { get; set; }
    public string Color { get; set; }
    public Boolean ClassesAssigned { get; set; }
  }
}