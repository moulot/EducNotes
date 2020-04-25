using System;
using System.Collections.Generic;

namespace EducNotes.API.Models {
    public class Order {
        public Order()
        {
          OrderDate = DateTime.Now;   
        }
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public int? ShippingAddressId { get; set; }
        public int? BillingAddressId { get; set; }
        public Address ShippingAddress { get; set; }
        public Address BillingAddress { get; set; }
        public decimal TotalHT { get; set; }
        public int Discount { get; set; }
        public decimal AmountHT { get; set; }
        public decimal AmountTTC { get; set; }
        public int TVA { get; set; }
        public int ChildId { get; set; }
        public User Child { get; set; }
        // IserP pour representer le parent . Il peut null car dans la pratique il n'est pas toujours renseigné
        public int? ParentId { get; set; }
        public User Parent { get; set; }

        public ICollection<OrderLine> OrderLines { get; set; }

    }
}