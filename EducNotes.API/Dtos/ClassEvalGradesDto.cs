namespace EducNotes.API.Dtos
{
    public class ClassEvalGradesDto
    {
        public int UserId { get; set; }
        public string PhotoUrl { get; set; }
        public string StudentName { get; set; }
        public string Grade { get; set; }
        public string Comment { get; set; }
    }
}