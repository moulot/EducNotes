using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
  public class ScheduleClassDto
  {
    public int ClassId { get; set; }
    public string ClassName { get; set; }
    public List<ScheduleDayDto> Days { get; set; }
  }
}