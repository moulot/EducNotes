namespace EducNotes.API.Dtos
{
    public class UsersRecapDto
    {
        public int UserTypeId { get; set; }
        public string  UserTypeName { get; set; }
        public int TotalAccount { get; set; }
        public int TotalActive { get; set; }
    }
}