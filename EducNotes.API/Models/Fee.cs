using System;

namespace EducNotes.API.Models
{
    public class Fee
    {
        public int Id { get; set; }
        public int FeeTypeId { get; set; }
        public FeeType FeeType { get; set; }
        public int StudentId { get; set; }
        public User Student { get; set; }
        public decimal Amount { get; set; }
        public DateTime FeeDate { get; set; }
        public DateTime DueDate { get; set; }
        public string Desc { get; set; }
    }
}