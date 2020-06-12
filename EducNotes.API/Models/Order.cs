using System;
using System.Collections.Generic;
using EducNotes.API.Dtos;

namespace EducNotes.API.Models {
  public class Order {
    public Order()
    {
      OrderDate = DateTime.Now;
      Status = 0;
      TotalHT = 0;
      Discount = 0;
      AmountHT = 0;
      AmountTTC = 0;
      TVA = 0;
      TVAAmount = 0;
      isReg = false;
      isNextReg = false;
    }

    public enum StatusEnum
    {
      Created = 0,
      ValidatedByClient = 1,
      Expired = 2,
      Cancelled = 3,
      Paid = 4,
      OverDue = 5,
      Completed = 6
    }

    public int Id { get; set; }
    public int OrderNum { get; set; }
    public string OrderLabel { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime Deadline { get; set; }
    public DateTime Validity { get; set; }
    public int? ShippingAddressId { get; set; }
    public Address ShippingAddress { get; set; }
    public int? BillingAddressId { get; set; }
    public Address BillingAddress { get; set; }
    public decimal TotalHT { get; set; }
    public decimal Discount { get; set; }
    public decimal AmountHT { get; set; }
    public decimal TVA { get; set; }
    public decimal TVAAmount { get; set; }
    public decimal AmountTTC { get; set; }
    public int? ChildId { get; set; }
    public User Child { get; set; }
    public int? FatherId { get; set; }
    public User Father { get; set; }
    public int? MotherId { get; set; }
    public User Mother { get; set; }
    public byte Status { get; set; }
    public Boolean isReg { get; set; }
    public Boolean isNextReg { get; set; }
    public List<OrderLine> Lines { get; set; }
  }
}