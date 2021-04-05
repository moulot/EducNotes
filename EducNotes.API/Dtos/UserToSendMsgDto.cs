namespace EducNotes.API.Dtos
{
  public class UserToSendMsgDto
  {
    public int Id { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public int Gender { get; set; }
    public string Email { get; set; }
    public string Mobile { get; set; }
    public int msgChoice { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public int ChildId { get; set; }
    public string Token { get; set; }
  }
}