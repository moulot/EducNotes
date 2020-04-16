namespace EducNotes.API.Models
{
    public class LessonContentDoc
    {
        public int Id { get; set; }
        public int LessonContentId { get; set; }
        public LessonContent LessonContent { get; set; }

          public int LessonDocId { get; set; }
        public LessonDoc LessonDoc { get; set; }
    }
}