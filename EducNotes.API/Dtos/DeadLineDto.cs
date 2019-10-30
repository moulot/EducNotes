using System;

namespace EducNotes.API.Dtos
{
    public class DeadLineDto
    {
        public string Name { get; set; }
        public string Comment { get; set; }
        public decimal Amount { get; set; }
        public string DueDate { get; set; }
    }
}