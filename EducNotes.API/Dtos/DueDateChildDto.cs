using System;
using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
  public class DueDateChildDto
  {
    public DueDateChildDto()
    {
      Products = new List<DueDateProductDto>();
    }

    public int ChildId { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public string PhotoUrl { get; set; }
    public string LevelName { get; set; }
    public string ClassName { get; set; }
    public decimal LateDueAmount { get; set; }
    public Boolean IsLate { get; set; }
    public List<DueDateProductDto> Products { get; set; }
  }
}