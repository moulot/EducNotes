using System;

namespace EducNotes.API.Helpers
{
    public class ScheduleParams
    {
        public int ClassId { get; set; }
        public int CourseId { get; set; }
        public int MoveWeek { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DueDate { get; set; }
    }
}