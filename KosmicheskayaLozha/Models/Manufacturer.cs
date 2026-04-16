using System.Collections.Generic;

namespace KosmicheskayaLozha.Models
{
    public class Manufacturer
    {
        public int ManufacturerId { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}