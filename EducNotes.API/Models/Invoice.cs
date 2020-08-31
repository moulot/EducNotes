using System;

namespace EducNotes.API.Models
{
  public class Invoice
  {
    public Invoice()
    {
      Created = false;
      Cancelled = false;
      Validated = false;
      Overdue = false;
      Paid = false;
    }

    public int Id { get; set; }
    public string InvoiceNum { get; set; }
    public decimal Amount { get; set; }
    public DateTime InvoiceDate { get; set; }
    public int? OrderId { get; set; }
    public Order Order { get; set; }
    public int? UserId { get; set; }
    public User User { get; set; }
    public Boolean Created { get; set; }
    public Boolean Cancelled { get; set; }
    public Boolean Validated { get; set; }
    public Boolean Overdue { get; set; }
    public Boolean Paid { get; set; }
  }
}