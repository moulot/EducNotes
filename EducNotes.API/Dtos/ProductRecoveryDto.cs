namespace EducNotes.API.Dtos
{
  public class ProductRecoveryDto
  {
    public string ProductName { get; set; }
    public decimal LateAmount { get; set; }
    public decimal LateAmount7Days { get; set; }
    public decimal LateAmount15Days { get; set; }
    public decimal LateAmount30Days { get; set; }
    public decimal LateAmount60Days { get; set; }
    public decimal LateAmount60DaysPlus { get; set; }
    public int NbDaysLate { get; set; }
  }
}