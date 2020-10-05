using System;

namespace EducNotes.API.Models
{
  public class FinOpOrderLine
  {
    public FinOpOrderLine()
    {
      InsertDate = DateTime.Now;
      InsertUserId = 1;
      UpdateDate = DateTime.Now;
      UpdateUserId = 1;
    }
    public int Id { get; set; }
    public int? InvoiceId { get; set; }
    public Invoice Invoice { get; set; }
    public int FinOpId { get; set; }
    public FinOp FinOp { get; set; }
    public int OrderLineId { get; set; }
    public OrderLine OrderLine { get; set; }
    public decimal Amount { get; set; }
    public DateTime InsertDate { get; set; }
    public int InsertUserId { get; set; }
    public User InsertUser { get; set; }
    public DateTime UpdateDate { get; set; }
    public int UpdateUserId { get; set; }
    public User UpdateUser { get; set; }
  }
}