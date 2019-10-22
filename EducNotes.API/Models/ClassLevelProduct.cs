namespace EducNotes.API.Models
{
    public class ClassLevelProduct
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public int ClasssLevelId { get; set; }
        public ClassLevel ClasssLevel { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }

    }
}