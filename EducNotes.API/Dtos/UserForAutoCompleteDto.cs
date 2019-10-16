namespace EducNotes.API.Dtos
{
    public class UserForAutoCompleteDto
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ClassName { get; set; }
        public string UserTypeName { get; set; }
        public string Email { get; set; }
        public string EmailConfirmed { get; set; }
    }
}