using System;

namespace EducNotes.API.Models
{
    public class DeadLine
    {   
        public DeadLine()
        {
            InsertDate = DateTime.Now;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public decimal Percentage { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime InsertDate { get; set; }
      
    }
}