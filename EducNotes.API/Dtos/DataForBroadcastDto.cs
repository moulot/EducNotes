using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
  public class DataForBroadcastDto
  {
    public int MsgType { get; set; }
    public List<int> UserTypeIds { get; set; }
    public List<int> ClassIds { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public int TemplateId { get; set; }
  }
}