namespace EducNotes.API.Models
{
    public class Lesson
    {
        public int Id { get; set; }
        public int? ClassLevelId { get; set; }
        public ClassLevel ClassLevel { get; set; }
        public int? CourseId { get; set; }
        public Course Course { get; set; }
        public int? ThemeId { get; set; }
        public Theme Theme { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
    }
}