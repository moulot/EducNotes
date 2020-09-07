namespace EducNotes.API.Dtos
{
  public class PaymentDto
  {
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public int FinOpId { get; set; }
    public int OrderLineId { get; set; }
    public string Child { get; set; }
    public decimal Amount { get; set; }
  }
}