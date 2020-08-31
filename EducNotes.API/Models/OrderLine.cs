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
      Validated = false;
      Paid = false;
      Expired = false;
      OverDue = false;
      Cancelled = false;
      Completed = false;
    }

    public int Id { get; set; }
    public int OrderId { get; set; }
    public Order Order { get; set; }
    public string OrderLineLabel { get; set; }
    public int? ClassLevelId { get; set; }
    public ClassLevel ClassLevel { get; set; }
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
    public Boolean Validated { get; set; }
    public Boolean Paid { get; set; }
    public Boolean Expired { get; set; }
    public Boolean OverDue { get; set; }
    public Boolean Cancelled { get; set; }
    public Boolean Completed { get; set; }
    public List<OrderLineDeadline> Deadlines { get; set; }
  }
}