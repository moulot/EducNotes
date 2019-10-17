namespace EducNotes.API.Dtos
{
    public class DataForEmail
    {
        public string Tos { get; set; }
        public string Ccs { get; set; }
        public string Bccs { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}