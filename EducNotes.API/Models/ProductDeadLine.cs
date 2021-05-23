using System;

namespace EducNotes.API.Models
{
    public class ProductDeadLine
    {
      public ProductDeadLine()
      {
        Seq = 0;
      }
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public DateTime DueDate { get; set; }
        public string DeadLineName { get; set; }
        public string Comment { get; set; }
        public byte Seq { get; set; }
        public decimal Percentage { get; set; }
        
    }
}