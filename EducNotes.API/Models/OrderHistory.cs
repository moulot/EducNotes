using System;

namespace EducNotes.API.Models
{
  public class OrderHistory
  {
    public int Id { get; set; }
    public int OrderId { get; set; }
    public Order Order { get; set; }
    public DateTime OpDate { get; set; }
    public string Action { get; set; }
    public decimal OldAmount { get; set; }
    public decimal NewAmount { get; set; }
    public decimal Delta { get; set; }
  }
}