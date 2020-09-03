using System;
using System.Collections.Generic;
using EducNotes.API.Models;

namespace EducNotes.API.Dtos
{
  public class OrderLineDto
  {
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string OrderLineLabel { get; set; }
    public DateTime Deadline { get; set; }
    public string strDeadline { get; set; }
    public int NbDaysLate { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public int Qty { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalHT { get; set; }
    public decimal Discount { get; set; }
    public decimal AmountHT { get; set; }
    public string strAmountHT { get; set; }
    public decimal TVA { get; set; }
    public decimal TVAAmount { get; set; }
    public decimal AmountTTC { get; set; }
    public string strAmountTTC { get; set; }
    public int ChildId { get; set; }
    public string ChildFirstName { get; set; }
    public string ChildLastName { get; set; }
    public string ChildAge { get; set; }
    public string ChildPhotoUrl { get; set; }
    public string ClassLevelName { get; set; }
    public int ChildClassId { get; set; }
    public string ChildClassName { get; set; }
    public byte Validated { get; set; }
    public byte Paid { get; set; }
    public byte Expired { get; set; }
    public byte Overdue { get; set; }
    public byte Cancelled { get; set; }
    public byte Completed { get; set; }
    public List<OrderLineDeadlineDto> Payments { get; set; }
  }
}