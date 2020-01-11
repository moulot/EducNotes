namespace EducNotes.API.Dtos
{
    public class CallBackDto
    {
        public string IntegrationName { get; set; }
        public string MessageId { get; set; }
        public string RequestId { get; set; }
        public string ClientMessageId { get; set; }
        public string To { get; set; }
        public string From { get; set; }
        public int StatusCode { get; set; }
        public string Status { get; set; }
        public string StatusDescription { get; set; }
        public string TimeStamp { get; set; }
    }
}