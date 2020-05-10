using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
  public class ChildRegistrationDto
  {
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string NextClass { get; set; }
    public string RegistrationFee { get; set; }
    public string TuitionAmount { get; set; }
    public string DueAmountPct { get; set; }
    public string DueAmount { get; set; }
    public string DueDate { get; set; }
    public string TotalDueForChild { get; set; }
  }
}