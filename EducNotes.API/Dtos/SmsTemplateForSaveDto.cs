namespace EducNotes.API.Dtos
{
    public class SmsTemplateForSaveDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public int SmsCategoryId { get; set; }
    }
}