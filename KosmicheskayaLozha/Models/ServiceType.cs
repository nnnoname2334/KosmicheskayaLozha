using System.Collections.Generic;

namespace KosmicheskayaLozha.Models
{
    public class ServiceType
    {
        public int ServiceTypeId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }

        public ICollection<MasterService> MasterServices { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
        public string PriceText { get; set; }
    }
}