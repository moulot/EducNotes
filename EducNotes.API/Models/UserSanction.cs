using System;

namespace EducNotes.API.Models
{
    public class UserSanction
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int SanctionId { get; set; }
        public Sanction Sanction { get; set; }
        public int SanctionedById { get; set; }
        public User SanctionedBy { get; set; }
        public DateTime SanctionDate { get; set; }
        public string Reason { get; set; }
        public string Comment { get; set; }
    }

}