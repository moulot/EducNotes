using System;

namespace EducNotes.API.Models
{
    public class Period
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public byte Active { get; set; }
    }
}