namespace EducNotes.API.Models
{
    public class ProductDeadLine
    {
        public int id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int DeadLineId { get; set; }
        public DeadLine DeadLine { get; set; }
        public decimal Percentage { get; set; }
        
    }
}