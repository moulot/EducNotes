using System;

namespace EducNotes.API.Models
{
    public class Inscription
    {
        public int Id { get; set; }
        public DateTime InsertDate { get; set; }
        public int ClassLevelId { get; set; }
        public int UserId { get; set; }
        public bool Validated { get; set; }
        public DateTime? ValidatedDate { get; set; }
        public User User { get; set; }
        public ClassLevel ClassLevel { get; set; }
        public int InscriptionTypeId { get; set; }
        public InscriptionType InscriptionType { get; set; }
        public int InsertUserId { get; set; }
        public User InsertUser { get; set; }
    }
}