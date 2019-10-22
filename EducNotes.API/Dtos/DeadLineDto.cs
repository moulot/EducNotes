using System;

namespace EducNotes.API.Dtos
{
    public class DeadLineDto
    {
        public string Name { get; set; }
        public string Comment { get; set; }
        public decimal Percentage { get; set; }
        public DateTime DueDate { get; set; }
    }
}