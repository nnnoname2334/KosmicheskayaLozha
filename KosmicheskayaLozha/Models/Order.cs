using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

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

        
        public string OrderDateFormatted { get; set; }

     
        public string DeliveryDateFormatted { get; set; }

    
        public string TotalText { get; set; }
    }
}