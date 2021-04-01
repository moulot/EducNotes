using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
  public class MsgRecipientsDto
  {
    public List<UserForDetailedDto> UsersOK { get; set;}
    public List<UserForDetailedDto> UsersNOK { get; set; }
  }
}