using System.Collections.Generic;
using EducNotes.API.Models;

namespace EducNotes.API.Dtos
{
  public class TuitionFeeDto
  {
    public TuitionFeeDto()
    {
      LevelFees = new List<LevelProductPriceDto>();
    }
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public List<LevelProductPriceDto> LevelFees { get; set; }
  }
}