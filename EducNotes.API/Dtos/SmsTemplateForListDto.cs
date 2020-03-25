namespace EducNotes.API.Dtos
{
    public class SmsTemplateForListDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public string SmsCategoryName { get; set; }
    }
}