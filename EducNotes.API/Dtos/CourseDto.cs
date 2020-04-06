using System;
using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
  public class CourseDto
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Abbreviation { get; set; }
    public string Color { get; set; }
    public Boolean ClassesAssigned { get; set; }
  }
}