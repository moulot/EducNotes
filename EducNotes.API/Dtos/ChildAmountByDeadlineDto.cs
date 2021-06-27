using System;

namespace EducNotes.API.Dtos
{
  public class ChildAmountByDeadlineDto
  {
    public int ChildId { get; set; }
    public string ChildFirstName { get; set; }
    public string ChildLastName { get; set; }
    public DateTime DueDate { get; set; }
    public string strDueDate { get; set; }
    public decimal Invoiced { get; set; }
    public decimal Paid { get; set; }
    public decimal Balance { get; set; }
    public Boolean IsLate { get; set; }
  }
}