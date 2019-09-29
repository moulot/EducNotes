using System;

namespace EducNotes.API.Models
{
    public class Session
    {
        public int Id { get; set; }
        public int ScheduleId { get; set; }
        public Schedule Schedule { get; set; }
        public DateTime SessionDate { get; set; }
        public string Comment { get; set; }
    }
}