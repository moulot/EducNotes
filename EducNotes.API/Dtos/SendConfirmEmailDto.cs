using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
    public class SendConfirmEmailDto
  {
    public int UserTypeId { get; set; }
    public List<int> UserIds { get; set; }
  }
}