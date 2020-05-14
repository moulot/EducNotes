using System;

namespace EducNotes.API.Models
{
  public class OrderLineDeadline
  {
    public int Id { get; set; }
    public int OrderLineId { get; set; }
    public OrderLine OrderLine { get; set; }
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }
    public string DeadlineName { get; set; }
    public string Comment { get; set; }
  }
}