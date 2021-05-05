using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
  public class RoleWithUsersDto
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public List<UserInRole> Users { get; set; }
  }
}