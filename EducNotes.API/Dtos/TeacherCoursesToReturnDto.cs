using System;

namespace EducNotes.API.Dtos
{
    public class TeacherCoursesToReturnDto
    {
        public int TeacherId { get; set; }
        public string TeacherName { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public int Day { get; set; }
        public DateTime StartHourMin { get; set; }
        public DateTime EndHourMin { get; set; }
    }
}