using System;

namespace EducNotes.API.Dtos
{
  public class FinOpForUpdateStatusDto
  {
    public int Id { get; set; }
    public Boolean Received { get; set; }
    public Boolean DepositedToBank { get; set; }
    public Boolean Rejected { get; set; }
    public Boolean Cashed { get; set; }
  }
}