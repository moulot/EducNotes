using System;
using System.Collections.Generic;

namespace EducNotes.API.Models
{
  public class OrderLine
  {
    public OrderLine()
    {
      Qty = 1;
      Discount = 0;
      TVA = 0;
      Status = 0;
      Cancelled = false;
    }

    public enum StatusEnum
    {
      ProductFee = 0,
      Created = 0,
      Shipped = 1,
      Completed = 2
    }

    public int Id { get; set; }
    public int OrderId { get; set; }
    public Order Order { get; set; }
    public string OrderLineLabel { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; }
    public decimal ProductFee { get; set; }
    public int Qty { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalHT { get; set; }
    public decimal Discount { get; set; }
    public decimal AmountHT { get; set; }
    public decimal TVA { get; set; }
    public decimal TVAAmount { get; set; }
    public decimal AmountTTC { get; set; }
    public int? ChildId { get; set; }
    public User Child { get; set; }
    public byte Status { get; set; }
    public Boolean Cancelled { get; set; }
    public List<OrderLineDeadline> Deadlines { get; set; }
  }
}