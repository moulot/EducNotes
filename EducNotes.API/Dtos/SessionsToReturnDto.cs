using System;

namespace EducNotes.API.Dtos
{
    public class SessionsToReturnDto
    {
        public int? TeacherId { get; set; }
        public string TeacherName { get; set; }
        public int? CourseId { get; set; }
        public string CourseName { get; set; }
        public string CourseColor { get; set; }
        public int? ClassId { get; set; }
        public string ClassName { get; set; }
        public int Day { get; set; }
        public string strDayDate { get; set; }
        public DateTime DayDate { get; set; }
        public string StartHourMin { get; set; }
        public string EndHourMin { get; set; }
    }
}