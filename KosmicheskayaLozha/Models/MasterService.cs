namespace KosmicheskayaLozha.Models
{
    public class MasterService
    {
        public int MasterServiceId { get; set; }
        public int MasterId { get; set; }
        public int ServiceTypeId { get; set; }

        public User Master { get; set; }
        public ServiceType ServiceType { get; set; }
    }
}