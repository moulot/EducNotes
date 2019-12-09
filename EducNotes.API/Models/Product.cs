namespace EducNotes.API.Models {
    public class Product {
        public Product()
        {
            IsRequired = false;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public int ProductTypeId { get; set; }
        public decimal? Price { get; set; }
        public bool IsByLevel { get; set; }
        public bool IsPeriodic { get; set; }
        public ProductType ProductType { get; set; }
        public int? PeriodicityId { get; set; }
        public Periodicity Periodicity { get; set; }
        public int? PayableAtId { get; set; }
        public PayableAt PayableAt { get; set; }
        public bool IsRequired { get; set; }
    }
}