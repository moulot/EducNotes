namespace EducNotes.API.Dtos
{
    public class UserSanctionsToReturnDto
    {
        public string UserName { get; set; }
        public string SanctionName { get; set; }
        public string SanctionedBy { get; set; }
        public string SanctionDate { get; set; }
        public string Reason { get; set; }
        public string Comment { get; set; }
    }
}