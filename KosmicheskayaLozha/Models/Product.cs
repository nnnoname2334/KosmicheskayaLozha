namespace KosmicheskayaLozha.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public int Discount { get; set; }
        public int ManufacturerId { get; set; }
        public int ProductTypeId { get; set; }
        public decimal Rating { get; set; }
        public bool IsFrozen { get; set; }

        public Manufacturer Manufacturer { get; set; }
        public ProductType ProductType { get; set; }

        // Цена с учётом скидки
        public decimal FinalPrice => Price * (1 - Discount / 100m);
        // Нужна ли подсветка
        public bool HasBigDiscount => Discount > 15;
        public string PriceText { get; set; }
        public string DiscountText { get; set; }
        public string FrozenText { get; set; }
        public string FreezeButtonText { get; set; }
    }
}