using System;

namespace EducNotes.API.Dtos
{
  public class TuitionChildDataDto
  {
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public byte Sex { get; set; }
    public DateTime DateOfBirth { get; set; }
    public int ClassLevelId { get; set; }
    public decimal TuitionFee { get; set; }
    public decimal RegFee { get; set; }
    public decimal DownPayment { get; set; }
  }
}