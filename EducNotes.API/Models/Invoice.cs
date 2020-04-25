using System;

namespace EducNotes.API.Models
{
    public class Invoice
    {
        public int Id { get; set; }
        public string InvoiceNum { get; set; }
        public DateTime InvoiceDate { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}