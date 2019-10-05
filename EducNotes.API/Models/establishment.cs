using System;

namespace EducNotes.API.Models
{
    public class Establishment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Phone { get; set; }
        public string WebSite { get; set; }
        public string Email { get; set; }
        public DateTime StartCoursesHour { get; set; }
        public DateTime EndCoursesHour { get; set; }
    }
}