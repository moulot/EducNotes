namespace EducNotes.API.Dtos
{
    public class ClassScheduleForTimeTableDto
    {
        public int Id { get; set; }
        public int ClassLevelId { get; set; }
        public string ClassLevelName { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string ClassLevel { get; set; }
        public int Day { get; set; }
        public string StartHourMin { get; set; }
        public string EndHourMin { get; set; }
        public string Top { get; set; }
        public string Height { get; set; }
        public string Color { get; set; }
    }
}