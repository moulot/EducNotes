using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
  public class ScheduleDueDateFeeDto
  {
    public List<ClasslevelProductDto> LevelProducts { get; set; }
    public List<ProductDeadlineDto> ProductDeadlines { get; set; }
  }
}