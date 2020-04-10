namespace EducNotes.API.Models
{
    public class LessonDocDocument
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public Document Document { get; set; }
        public int LessonDocId { get; set; }
        public LessonDoc LessonDoc { get; set; }
    }
}