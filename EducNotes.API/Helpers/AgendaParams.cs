using System;

namespace EducNotes.API.Helpers
{
    public class AgendaParams
    {
        public int ClassId { get; set; }
        public int CourseId { get; set; }
        public int MoveWeek { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime CurrentDate { get; set; }
        public int nbDays { get; set; }
        public bool IsMovingPeriod { get; set; }
    }
}