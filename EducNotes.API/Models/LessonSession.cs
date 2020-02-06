namespace EducNotes.API.Models
{
    public class LessonSession
    {
        public int Id { get; set; }
        public int SessionId { get; set; }
        public Session Session { get; set; }
        public int? LessonId { get; set; }
        public Lesson Lesson { get; set; }
        public int? LessonContentId { get; set; }
        public LessonContent LessonContent { get; set; }
        public string Comment { get; set; }
        public int HoursDone { get; set; }
    }
}