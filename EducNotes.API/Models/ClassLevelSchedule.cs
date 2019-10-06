using System;

namespace EducNotes.API.Models
{
    public class ClassLevelSchedule
    {
        public int Id { get; set; }
        public int ClassLevelId { get; set; }
        public ClassLevel ClassLevel { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public int Day { get; set; }
        public DateTime StartHourMin { get; set; }
        public DateTime EndHourMin { get; set; }
    }
}