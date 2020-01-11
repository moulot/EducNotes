namespace EducNotes.API.Models
{
    public class UserSmsTemplate
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int SmsTemplateId { get; set; }
        public SmsTemplate SmsTemplate { get; set; }
    }
}