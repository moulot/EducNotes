using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
    public class PeriodEvalsDto
    {
        public int PeriodId { get; set; }
        public string PeriodName { get; set; }
        public string PeriodAbbrev { get; set; }
        public bool Active { get; set; }
        public double UserCourseAvg { get; set; }
        public List<GradeDto> grades { get; set; }
        public double ClassCourseAvg { get; set; }
    }
}