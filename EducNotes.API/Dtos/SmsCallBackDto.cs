namespace EducNotes.API.Dtos
{
    public class SmsCallBackDto
    {
        public string integrationName { get; set; }
        public string messageId { get; set; }
        public string requestId { get; set; }
        public string clientMessageId { get; set; }
        public string to { get; set; }
        public string from { get; set; }
        public int statusCode { get; set; }
        public string status { get; set; }
        public string stautsDescription { get; set; }
        public string timestamp { get; set; }
    }
}