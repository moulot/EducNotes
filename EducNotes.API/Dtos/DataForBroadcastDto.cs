using System;
using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
  public class DataForBroadcastDto
  {
    public Boolean SendToUsersNOK { get; set; }
    public int MsgChoice { get; set; }
    public int MsgType { get; set; }
    public List<int> UserTypeIds { get; set; }
    public List<int> EducLevelIds { get; set; }
    public List<int> SchoolIds { get; set; }
    public List<int> ClassLevelIds { get; set; }
    public List<int> ClassIds { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public int TemplateId { get; set; }
  }
}