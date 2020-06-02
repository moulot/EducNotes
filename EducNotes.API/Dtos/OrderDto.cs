using System;
using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
  public class OrderDto
  {
    public int Id { get; set; }
    public int OrderNum { get; set; }
    public string OrderLabel { get; set; }
    public DateTime OrderDate { get; set; }
    public string strOrderDate { get; set; }
    public DateTime Deadline { get; set; }
    public string strDeadline { get; set; }
    public DateTime Validity { get; set; }
    public string strValidity { get; set; }
    public int? ShippingAddressId { get; set; }
    public int? BillingAddressId { get; set; }
    public decimal TotalHT { get; set; }
    public decimal Discount { get; set; }
    public decimal AmountHT { get; set; }
    public decimal TVA { get; set; }
    public decimal TVAAmount { get; set; }
    public decimal AmountTTC { get; set; }
    public string strAmountTTC { get; set; }
    public int ChildId { get; set; }
    public string ChildLastName { get; set; }
    public string ChildFirstName { get; set; }
    public int ChildClassId { get; set; }
    public string ChildClassName { get; set; }
    public int ParentId { get; set; }
    public string ParentLastName { get; set; }
    public string ParentFirstName { get; set; }
    public string ParentCell { get; set; }
    public string ParentEmail { get; set; }
    public byte Status { get; set; }
    public Boolean isReg { get; set; }
    public Boolean isNextReg { get; set; }
    public List<OrderLineDto> Lines { get; set; }
  }
}