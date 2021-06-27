using System;
using System.Collections.Generic;
using EducNotes.API.Models;

namespace EducNotes.API.Dtos
{
  public class DueDateWithLinesDto
  {
    public DateTime DueDate { get; set; }
    public string strDueDate { get; set; }
    public decimal Invoiced { get; set; }
    public decimal Paid { get; set; }
    public decimal Balance { get; set; }
    public Boolean IsLate { get; set; }
    public List<OrderLineDeadline> LineDeadlines { get; set; }
  }
}