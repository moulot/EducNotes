namespace EducNotes.API.Models
{
    public class OrderLine
    {
        public OrderLine()
        {
            Qty =1;
            Discount = 0;
            TVA = 0;
        }
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int Qty { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalHT { get; set; }
        public int Discount { get; set; }
        public decimal AmountHT { get; set; }
        public decimal TVA { get; set; }
        public decimal AmountTVA { get; set; }
        public decimal AmountTTC { get; set; }
    }
}