using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
    public class TeacherScheduleDto
    {
      public int TeacherId { get; set; }
      public string TeacherName { get; set; }
      public List<ScheduleClassDto> Classes { get; set; }
      public List<ScheduleDayDto> Days { get; set; }
    }
}