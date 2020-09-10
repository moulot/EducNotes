using System;

namespace EducNotes.API.Models
{
  public class OrderLineHistory
  {
    public int Id { get; set; }
    public int OrderLineId { get; set; }
    public OrderLine OrderLine { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public DateTime OpDate { get; set; }
    public string Action { get; set; }
    public decimal OldAmount { get; set; }
    public decimal NewAmount { get; set; }
    public decimal Delta { get; set; }
  }
}