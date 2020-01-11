using System;

namespace EducNotes.API.Models
{
    public class Sms
    {
        public int Id { get; set; }
        public string To { get; set; }
        public int ToUserId { get; set; }
        public User ToUser { get; set; }
        public string From { get; set; }
        public string Content { get; set; }
        public DateTime TimeToSend { get; set; }
        // public byte StatusFlag { get; set; }
        public DateTime InsertDate { get; set; }
        public int InsertUserId { get; set; }
        public User InsertUser { get; set; }
        public DateTime UpdateDate { get; set; }
        public int UpdateUserId { get; set; }
        public User UpdateUser { get; set; }
        public string res_ApiMsgId { get; set; }
        public Boolean res_accepted { get; set; }
        public int res_ErrorCode { get; set; }
        public string res_Error { get; set; }
        public string res_ErrorDesc { get; set; }
        public string cb_IntegrationName { get; set; }
        public int cb_StatusCode { get; set; }
        public string cb_Status { get; set; }
        public int cb_StatusDesc { get; set; }
        public string cb_TimeStamp { get; set; }
    }
}