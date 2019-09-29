using System;

namespace EducNotes.API.Dtos
{
    public class ScheduleToReturnDto
    {
        public int Id { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public int Day { get; set; }
        public DateTime StartHourMin { get; set; }
        public DateTime EndHourMin { get; set; }
        public string strStartHourMin { get; set; }
        public string strEndHourMin { get; set; }
    }
}