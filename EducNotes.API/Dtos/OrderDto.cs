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
    public int NbDaysLate { get; set; }
    public int? ShippingAddressId { get; set; }
    public int? BillingAddressId { get; set; }
    public decimal TotalHT { get; set; }
    public decimal Discount { get; set; }
    public decimal AmountHT { get; set; }
    public string strAmountHT { get; set; }
    public decimal AmountToValidate { get; set; }
    public string strAmountToValidate { get; set; }
    public decimal TVA { get; set; }
    public decimal TVAAmount { get; set; }
    public decimal AmountTTC { get; set; }
    public string strAmountTTC { get; set; }
    public decimal DownPayment { get; set; }
    public decimal AmountInvoiced { get; set; }
    public string strAmountInvoiced { get; set; }
    public decimal AmountPaid { get; set; }
    public string strAmountPaid { get; set; }
    public decimal DueAmount { get; set; }
    public decimal NextDueAmount { get; set; }
    public string strNextDeadline { get; set; }
    public decimal Balance { get; set; }
    public string strBalance { get; set; }
    public int ChildId { get; set; }
    public string ChildLastName { get; set; }
    public string ChildFirstName { get; set; }
    public int ChildClassId { get; set; }
    public string ChildClassName { get; set; }
    public string ChildLevelName { get; set; }
    public int FatherId { get; set; }
    public string FatherLastName { get; set; }
    public string FatherFirstName { get; set; }
    public string FatherCell { get; set; }
    public string FatherEmail { get; set; }
    public int MotherId { get; set; }
    public string MotherLastName { get; set; }
    public string MotherFirstName { get; set; }
    public string MotherCell { get; set; }
    public string MotherEmail { get; set; }
    public int NbPayRejected { get; set; }
    public byte Status { get; set; }
    public Boolean isReg { get; set; }
    public Boolean isNextReg { get; set; }
    public Boolean Created { get; set; }
    public Boolean Validated { get; set; }
    public Boolean Expired { get; set; }
    public Boolean Cancelled { get; set; }
    public Boolean Overdue { get; set; }
    public Boolean Paid { get; set; }
    public Boolean Completed { get; set; }
    public List<OrderLineDto> Lines { get; set; }
    public List<FinOpDto> Payments { get; set; }
    public List<PaymentDto> LinePayments { get; set; }
  }
}