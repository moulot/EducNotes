using System;

namespace EducNotes.API.Dtos
{
  public class DueDateProductDto
  {
    public string ProductName { get; set; }
    public decimal LateDueAmount { get; set; }
    public decimal Invoiced { get; set; }
    public decimal Paid { get; set; }
    public decimal Balance { get; set; }
    public Boolean IsLate { get; set; }
  }
}