namespace EducNotes.API.Models
{
    public class LessonContent
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public int LessonId { get; set; }
        public Lesson Lesson { get; set; }
        public int NbHours { get; set; }
        public byte? SessionNum { get; set; }
    }
}