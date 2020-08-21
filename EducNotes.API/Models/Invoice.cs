using System;

namespace EducNotes.API.Models
{
  public class Invoice
  {
    public Invoice()
    {
      Status = 0;
    }

    public enum StatusEnum
    {
      Created = 0,
      Cancelled = 1,
      Validated = 2,
      OverDue = 3,
      Paid = 4
    }

    public int Id { get; set; }
    public string InvoiceNum { get; set; }
    public decimal Amount { get; set; }
    public DateTime InvoiceDate { get; set; }
    public int? OrderId { get; set; }
    public Order Order { get; set; }
    public int? UserId { get; set; }
    public User User { get; set; }
    public byte Status { get; set; }
  }
}