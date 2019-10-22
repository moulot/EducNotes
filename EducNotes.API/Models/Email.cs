using System;

namespace EducNotes.API.Models
{
    public class Email
    {
        public Email()
        {
            TimeToSend = DateTime.Now;    
            InsertDate =DateTime.Now;
        }

        public int Id { get; set; }
        public int EmailTypeId { get; set; }
        public EmailType EmailType { get; set; }
        public string ToAddress { get; set; }
        public string CCAddress { get; set; }
        public string BCCAddress { get; set; }
        public string FromAddress { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime TimeToSend { get; set; }
        public byte StatusFlag { get; set; }
        public DateTime InsertDate { get; set; }
        public int InsertUserId { get; set; }
        public User InsertUser { get; set; }
        public DateTime UpdateDate { get; set; }
        public int UpdateUserId { get; set; }
        public User UpdateUser { get; set; }
    }
}