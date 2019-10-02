namespace EducNotes.API.Models
{
    public class UserLink
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int UserPId { get; set; }
        public User UserP { get; set; }
    }
}