using System.Collections.Generic;
using EducNotes.API.Models;

namespace EducNotes.API.Dtos
{
  public class MsgRecipientsDto
  {
    public List<User> UsersOK { get; set;}
    public List<User> UsersNOK { get; set; }
  }
}