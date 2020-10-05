using System;

namespace EducNotes.API.Dtos
{
  public class AmountWithDeadlinesDto
  {
    public DateTime DueDate { get; set; }
    public string strDueDate { get; set; }
    public decimal Invoiced { get; set; }
    public decimal Paid { get; set; }
    public decimal Balance { get; set; }
    public Boolean IsLate { get; set; }
  }
}
