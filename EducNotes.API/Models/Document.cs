namespace EducNotes.API.Models
{
    public class Document
    {
        public int Id { get; set; }
        public int DocTypeId { get; set; }
        public DocType DocType { get; set; }
        public int FileTypeId { get; set; }
        public FileType FileType { get; set; }
        public int? ClassId { get; set; }
        public Class Class { get; set; }
        public int? ClassLevelId { get; set; }
        public ClassLevel ClassLevel { get; set; }
        public int? SessionId { get; set; }
        public Session Session { get; set; }
        public int? StudentId { get; set; }
        public User Student { get; set; }
        public int? EvaluationId { get; set; }
        public Evaluation Evaluation { get; set; }
        public int? AuthorId { get; set; }
        public User Author { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string PublicId { get; set; }
    }
}