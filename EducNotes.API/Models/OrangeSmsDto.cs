namespace EducNotes.API.Models
{
  class SmsDataRequest
  {
    public OutboundSMSMessageRequest outboundSMSMessageRequest { get; set;}
  }

  class OutboundSMSMessageRequest
  {
    public string Address { get; set; }
    public string SenderAddress { get; set; }
    public OutboundSMSTextMessage outboundSMSTextMessage { get; set; }
  }

  class OutboundSMSTextMessage
  {
    public string Message { get; set; }
  }
}