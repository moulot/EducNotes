using System;

namespace EducNotes.API.Dtos
{
    public class UserSmsTemplateDto
    {
        public int UserId { get; set; }
        public int SmsTemplateId { get; set; }
        public string SmsName { get; set; }
        public string Content { get; set; }
        public int SmsCategoryId { get; set; }
        public Boolean Active { get; set; }
    }
}