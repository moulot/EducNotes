namespace EducNotes.API.Dtos
{
  public class PaymentDto
  {
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public string InvoiceNum { get; set; }
    public int FinOpId { get; set; }
    public int OrderLineId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public string ChildLastName { get; set; }
    public string ChildFirstName { get; set; }
    public decimal Amount { get; set; }
  }
}