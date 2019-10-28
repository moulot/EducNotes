using System;

namespace EducNotes.API.Models
{
    public class Invoice
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        // public int FeeId { get; set; }
        // public Fee Fee { get; set; }
        public string Num { get; set; }
        public DateTime OpDate { get; set; }
        public int OrderId { get; set; }
        public Order order { get; set; }
    }
}