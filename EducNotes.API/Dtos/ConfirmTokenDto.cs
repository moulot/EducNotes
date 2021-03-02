namespace EducNotes.API.Dtos
{
    public class ConfirmTokenDto
    {
      public string UserId { get; set; }
      public string UserName { get; set; }
      public string Token { get; set; }
      public string Password { get; set; }
      public string OldPassword { get; set; }
      public string PhoneNumber { get; set; }
      public string Email { get; set; }
    }
}