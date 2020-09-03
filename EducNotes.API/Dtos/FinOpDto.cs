using System;

namespace EducNotes.API.Dtos
{
  public class FinOpDto
  {
    public int Id { get; set; }
    public DateTime FinOpDate { get; set; }
    public string strFinOpDate { get; set; }
    public int OrderId { get; set; }
    public int InvoiceId { get; set; }
    public string InvoiceNum { get; set; }
    public DateTime InvoiceDate { get; set; }
    public string strInvoiceDate { get; set; }
    public decimal InvoiceAmount { get; set; }
    public int ChequeId { get; set; }
    public string ChequeNum { get; set; }
    public string ChequeBankName { get; set; }
    public decimal ChequeAmount { get; set; }
    public string ChequePictureUrl { get; set; }
    public int PaymentTypeId { get; set; }
    public string PaymentTypeName { get; set; }
    public decimal Amount { get; set; }
    public string strAmount { get; set; }
    public int? FromUserId { get; set; }
    public int? ForUserId { get; set; }
    public int? FromCashDeskId { get; set; }
    public string FromCashDeskName { get; set; }
    public int? FromBankAccountId { get; set; }
    public string FromBankAccountName { get; set; }
    public int? ToCashDeskId { get; set; }
    public string ToCashDeskName { get; set; }
    public int? ToBankAccountId { get; set; }
    public string ToBankAccountName { get; set; }
    public string DocRef { get; set; }
    public string Note { get; set; }
    public Boolean Received { get; set; }
    public Boolean DepositedToBank { get; set; }
    public Boolean Cashed { get; set; }
    public Boolean Rejected { get; set; }
  }
}