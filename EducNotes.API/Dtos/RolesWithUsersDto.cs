namespace EducNotes.API.Dtos
{
  public class RolesWithUsersDto
  {
    public int RoleId { get; set; }
    public int UserId { get; set; }
    public string RoleName { get; set; }
    public string UserFirstName { get; set; }
    public string UserLastName { get; set; }
    public string PhotoUrl { get; set; }
  }
}