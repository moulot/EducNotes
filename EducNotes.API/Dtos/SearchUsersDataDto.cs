namespace EducNotes.API.Dtos
{
  public class SearchUsersDataDto
  {
    public int UserId { get; set; }
    public int UserTypeId { get; set; }
    public string UserType { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string IDNum { get; set; }
    public int Age { get; set; }
    public string ClassName { get; set; }
    public string ClassLevelName { get; set; }
    public string PhotoUrl { get; set; }
  }
}