using System;
using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
  public class DueDateChildDto
  {
    public int ChildId { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public string PhotoUrl { get; set; }
    public string LevelName { get; set; }
    public string ClassName { get; set; }
    public DateTime DueDate { get; set; }
    public string strDueDate { get; set; }
    public decimal LateDueAmount { get; set; }
    public decimal Invoiced { get; set; }
    public decimal Paid { get; set; }
    public decimal Balance { get; set; }
    public Boolean IsLate { get; set; }
  }
}