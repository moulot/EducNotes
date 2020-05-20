using System;

namespace EducNotes.API.Dtos
{
  public class OrderlineToValidateDto
  {
    public int OrderlineId { get; set; }
    public Boolean Cancelled { get; set; }
  }
}