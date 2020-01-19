namespace EducNotes.API.Models
{
    public class UserSmsTemplate
    {
        public int Id { get; set; }
        public int ChildId { get; set; }
        public User Child { get; set; }
        public int ParentId { get; set; }
        public User Parent { get; set; }
        public int SmsTemplateId { get; set; }
        public SmsTemplate SmsTemplate { get; set; }
    }
}