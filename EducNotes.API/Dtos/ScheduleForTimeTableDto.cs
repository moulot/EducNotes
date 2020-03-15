namespace EducNotes.API.Dtos
{
    public class ScheduleForTimeTableDto
    {
        public int Id { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string ClassLevel { get; set; }
        public int? TeacherId { get; set; }
        public string TeacherName { get; set; }
        public int Day { get; set; }
        public string StartHourMin { get; set; }
        public string EndHourMin { get; set; }
        public string Top { get; set; }
        public string Height { get; set; }
        public string Color { get; set; }
        public string DelInfo { get; set; }
    }
}