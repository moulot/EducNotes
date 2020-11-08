using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
  public class UserTypeToValidateDto
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public List<UserToValidateDto> Users { get; set; }
  }
}