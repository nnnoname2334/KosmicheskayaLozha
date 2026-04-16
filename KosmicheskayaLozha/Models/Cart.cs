namespace KosmicheskayaLozha.Models
{
    public class Cart
    {
        public int CartId { get; set; }
        public int ClientId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        public User Client { get; set; }
        public Product Product { get; set; }
    }
}