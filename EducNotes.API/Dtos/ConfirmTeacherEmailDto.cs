namespace EducNotes.API.Dtos
{
  public class ConfirmTeacherEmailDto
  {
    public int Id { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public byte Gender { get; set; }
    public string Cell { get; set; }
    public string Email { get; set; }
    public string Token { get; set; }
    public string Courses { get; set; }
  }
}