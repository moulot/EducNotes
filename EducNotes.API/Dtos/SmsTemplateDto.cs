namespace EducNotes.API.Dtos
{
    public class SmsTemplateDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SmsCategoryId { get; set; }
        public string Content { get; set; }
    }
}