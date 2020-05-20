using System;

namespace EducNotes.API.Dtos
{
  public class OrderLineDto
  {
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string OrderLineLabel { get; set; }
    public string ProductName { get; set; }
    public int Qty { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalHT { get; set; }
    public decimal Discount { get; set; }
    public decimal AmountHT { get; set; }
    public decimal TVA { get; set; }
    public decimal TVAAmount { get; set; }
    public decimal AmountTTC { get; set; }
    public string strAmountTTC { get; set; }
    public string ChildFirstName { get; set; }
    public string ChildLastName { get; set; }
    public string ChildClassName { get; set; }
    public Boolean Cancelled { get; set; }
  }
}