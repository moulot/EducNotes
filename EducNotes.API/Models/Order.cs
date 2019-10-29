using System;
using System.Collections.Generic;

namespace EducNotes.API.Models {
    public class Order {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public int? ShippingAddressId { get; set; }
        public int? BillingAddressId { get; set; }

        public Address ShippingAddress { get; set; }
        public Address BillingAddress { get; set; }
        public decimal TotalHT { get; set; }
        public decimal TotalTTC { get; set; }
        public int Discount { get; set; }
        public int TVA { get; set; }
        public ICollection<OrderLine> OrderLines { get; set; }

    }
}