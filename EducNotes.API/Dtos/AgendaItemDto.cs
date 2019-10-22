namespace EducNotes.API.Dtos
{
    public class AgendaItemDto
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string CourseAbbrev { get; set; }
        public string CourseColor { get; set; }
        public string strDateAdded { get; set; }
        public string strDueDate { get; set; }
        public string TaskDesc { get; set; }
        public bool Done { get; set; }
    }
}