using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
  public class OrderToValidateDto
  {
    public int OrderId { get; set; }
    public List<OrderlineToValidateDto> OrderlineIds { get; set; }
  }
}