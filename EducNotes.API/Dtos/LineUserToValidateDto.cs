using System;

namespace EducNotes.API.Dtos
{
  public class LineUserToValidateDto
  {
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string strDeadline { get; set; }
    public int NbDaysLate { get; set; }
    public decimal TuitionAmount { get; set; }
    public string strAmountTTC { get; set; }
    public decimal DueAmount { get; set; }
    public int ChildId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Age { get; set; }
    public string PhotoUrl { get; set; }
    public string ClassLevelName { get; set; }
    public int ClassId { get; set; }
    public string ClassName { get; set; }
    public Boolean Validated { get; set; }
    public Boolean Paid { get; set; }
    public Boolean Overdue { get; set; }
    public Boolean Completed { get; set; }
  }
}