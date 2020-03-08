namespace EducNotes.API.Dtos
{
    public class ImportUserDto
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string PhoneNomber { get; set; }
        public int UserTypeId { get; set; }
        public string SecondPhoneNumber { get; set; }
        public string Email { get; set; }
        public int? MaxChild { get; set; }
    }
}