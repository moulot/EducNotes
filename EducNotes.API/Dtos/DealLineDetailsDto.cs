using System;

namespace EducNotes.API.Dtos {
    public class DealLineDetailsDto {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public decimal Amount { get; set; }
        public string DueDate { get; set; }
        public DateTime InsertDate { get; set; }
        public bool isByAmount { get; set; }
    }
}