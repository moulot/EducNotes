namespace EducNotes.API.Dtos
{
  public class OrderLinePaidDto
  {
    public int OrderLineId { get; set; }
    public int ChildId { get; set; }
    public int ProductId { get; set; }
    public decimal Amount { get; set; }
  }
}