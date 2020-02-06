namespace EducNotes.API.Models
{
    public class SessionDocument
    {
        public int Id { get; set; }
        public int SessionId { get; set; }
        public Session Session { get; set; }
        public int DocumentId { get; set; }
        public Document Document { get; set; }
    }
}