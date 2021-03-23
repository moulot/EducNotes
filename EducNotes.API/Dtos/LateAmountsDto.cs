using System;

namespace EducNotes.API.Dtos
{
  public class LateAmountsDto
  {
    public Decimal TotalLateAmount { get; set; }
    public Decimal LateAmount7Days { get; set; }
    public Decimal LateAmount15Days { get; set; }
    public Decimal LateAmount30Days { get; set; }
    public Decimal LateAmount60Days { get; set; }
    public Decimal LateAmount60DaysPlus { get; set; }
  }
}