namespace EducNotes.API.Models
{
    public class ProductDeadLine
    {
        public int id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int DeadLineid { get; set; }
        public DeadLine DeadLine { get; set; }
    }
}