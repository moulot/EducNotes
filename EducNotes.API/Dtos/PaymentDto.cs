using System;

namespace EducNotes.API.Dtos
{
  public class PaymentDto
  {
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public int FinOpTypeId { get; set; }
    public string InvoiceNum { get; set; }
    public int PaymentTypeId { get; set; }
    public string TypeName { get; set; }
    public string ChequeNum { get; set; }
    public string ChequeBank { get; set; }
    public string FromBankAccount { get; set; }
    public string FromCashDesk { get; set; }
    public string ToBankAccount { get; set; }
    public string ToCashDesk { get; set; }
    public int FinOpId { get; set; }
    public string strFinOpDate { get; set; }
    public int OrderLineId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public string ChildLastName { get; set; }
    public string ChildFirstName { get; set; }
    public decimal Amount { get; set; }
    public Boolean Cashed { get; set; }
    public Boolean Received { get; set; }
    public Boolean DepositedToBank { get; set; }
    public Boolean Rejected { get; set; }
  }
}