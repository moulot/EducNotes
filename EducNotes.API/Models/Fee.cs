using System;

namespace EducNotes.API.Models
{
    public class Fee
    {
        public int Id { get; set; }
        public decimal Percentage { get; set; }
        public DateTime Duedate { get; set; }
        // public int DeadLineId { get; set; }
        // public DeadLine DeadLine { get; set; }
        // public int ClassLevelFeeId { get; set; }
        // public ClassLevelFee ClassLevelFee { get; set; }
    }
}