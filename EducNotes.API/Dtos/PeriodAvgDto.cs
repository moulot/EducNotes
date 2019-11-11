using System;

namespace EducNotes.API.Dtos
{
    public class PeriodAvgDto
    {
        public int PeriodId { get; set; }
        public string PeriodName { get; set; }
        public string PeriodAbbrev { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool Active { get; set; }
        public bool activated { get; set; }
        public double Avg { get; set; }
    }
}