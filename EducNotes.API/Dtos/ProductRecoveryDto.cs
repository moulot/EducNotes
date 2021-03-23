namespace EducNotes.API.Dtos
{
  public class ProductRecoveryDto
  {
    public string ProductName { get; set; }
    public int NbDaysLate { get; set; }
    public LateAmountsDto LateAmounts { get; set; }
  }
}