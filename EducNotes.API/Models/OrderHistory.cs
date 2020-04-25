using System;

namespace EducNotes.API.Models
{
  public class OrderHistory
  {
    public int Id { get; set; }
    public int OrderId { get; set; }
    public Order Order { get; set; }
    public int ChildId { get; set; }
    public User Child { get; set; }
    public int? ParentId { get; set; }
    public DateTime OpDate { get; set; }
    public User Parent { get; set; }
    public string Action { get; set; }
    public decimal OldAmount { get; set; }
    public decimal NewAmount { get; set; }
    public decimal Amount_Delta { get; set; }
  }
}