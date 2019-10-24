namespace EducNotes.API.Dtos
{
    public class CourseTasksDto
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string CourseAbbrev { get; set; }
        public string CourseColor { get; set; }
        public int NbTasks { get; set; }
    }
}