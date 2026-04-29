using System;
using System.ComponentModel.DataAnnotations.Schema;
namespace KosmicheskayaLozha.Models
{
    public class Appointment
    {
        public int AppointmentId { get; set; }
        public int? ClientId { get; set; }
        public int MasterId { get; set; }
        public int ServiceTypeId { get; set; }
        public DateTime DateTime { get; set; }
        public string PaymentMethod { get; set; }
        public string Comment { get; set; }
        public string Status { get; set; }

        public User Client { get; set; }
        public User Master { get; set; }
        public ServiceType ServiceType { get; set; }


        public string DateTimeText { get; set; }
        
        public string PriceText { get; set; }
       
        public string DateTimeFormatted { get; set; }

        
        public string StatusColor { get; set; }
    }
}