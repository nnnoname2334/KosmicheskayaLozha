using System.Collections.Generic;

namespace KosmicheskayaLozha.Models
{
    public class ProductType
    {
        public int ProductTypeId { get; set; }
        public string Name { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}