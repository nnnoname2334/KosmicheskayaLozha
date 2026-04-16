using System;
using System.Collections.Generic;

namespace KosmicheskayaLozha.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int ClientId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }

        public User Client { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}