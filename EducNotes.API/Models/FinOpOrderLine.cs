namespace EducNotes.API.Models
{
  public class FinOpOrderLine
  {
    public int Id { get; set; }
    public int? InvoiceId { get; set; }
    public Invoice Invoice { get; set; }
    public int FinOpId { get; set; }
    public FinOp FinOp { get; set; }
    public int OrderLineId { get; set; }
    public OrderLine OrderLine { get; set; }
    public decimal Amount { get; set; }
  }
}