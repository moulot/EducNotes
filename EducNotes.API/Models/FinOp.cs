using System;

namespace EducNotes.API.Models
{
    public class FinOp
    {
        public int Id { get; set; }
        public int FromUserId { get; set; }
        public User FromUser { get; set; }
        public int ForUserId { get; set; }
        public User ForUser { get; set; }
        public int PaymentTypeId { get; set; }
        public PaymentType PaymentType { get; set; }
        public int ToCashDeskId { get; set; }
        public CashDesk ToCashDesk { get; set; }
        public int ToBankAccountId { get; set; }
        public BankAccount ToBankAccount { get; set; }
        public decimal Amount { get; set; }
        public DateTime OpDate { get; set; }
        public string DocRef { get; set; }
        public string Note { get; set; }
        public byte Status { get; set; }
        public int InvoiceId { get; set; }
        public Invoice Invoice { get; set; }
    }
}