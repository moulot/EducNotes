using System;

namespace EducNotes.API.Models
{
  public class FinOp
  {
    public FinOp()
    {
      Received = false;
      DepositedToBank = false;
      Cashed = false;
      Rejected = false;
      InsertDate = DateTime.Now;
      InsertUserId = 1;
      UpdateDate = DateTime.Now;
      UpdateUserId = 1;
  }

    public int Id { get; set; }
    public DateTime FinOpDate { get; set; }
    public int? FinOpTypeId { get; set; }
    public FinOpType FinOpType { get; set; }
    public int? OrderId { get; set; }
    public Order Order { get; set; }
    public int? InvoiceId { get; set; }
    public Invoice Invoice { get; set; }
    public int? ChequeId { get; set; }
    public Cheque Cheque { get; set; }
    public int PaymentTypeId { get; set; }
    public PaymentType PaymentType { get; set; }
    public decimal Amount { get; set; }
    public int? FromUserId { get; set; }
    public User FromUser { get; set; }
    public int? ForUserId { get; set; }
    public User ForUser { get; set; }
    public int? FromBankId { get; set; }
    public Bank FromBank { get; set; }
    public int? FromCashDeskId { get; set; }
    public CashDesk FromCashDesk { get; set; }
    public int? FromBankAccountId { get; set; }
    public BankAccount FromBankAccount { get; set; }
    public int? ToCashDeskId { get; set; }
    public CashDesk ToCashDesk { get; set; }
    public int? ToBankAccountId { get; set; }
    public BankAccount ToBankAccount { get; set; }
    public string DocRef { get; set; }
    public string Note { get; set; }
    public Boolean Received { get; set; }
    public Boolean DepositedToBank { get; set; }
    public Boolean Cashed { get; set; }
    public Boolean Rejected { get; set; }
    public DateTime InsertDate { get; set; }
    public int InsertUserId { get; set; }
    public User InsertUser { get; set; }
    public DateTime UpdateDate { get; set; }
    public int UpdateUserId { get; set; }
    public User UpdateUser { get; set; }
  }
}