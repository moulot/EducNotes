using System;

namespace EducNotes.API.Models
{
  public class OrderLineDeadline
  {
    public OrderLineDeadline()
    {
      ProductFee = 0;
    }
    
    public int Id { get; set; }
    public int OrderLineId { get; set; }
    public OrderLine OrderLine { get; set; }
    public decimal Percent { get; set; }
    public decimal Amount { get; set; }
    public decimal ProductFee { get; set; }
    public DateTime DueDate { get; set; }
    public string DeadlineName { get; set; }
    public string Comment { get; set; }
    public byte Seq { get; set; }
    public Boolean Paid { get; set; }
  }
}