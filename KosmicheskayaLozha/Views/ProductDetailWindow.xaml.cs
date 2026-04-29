using KosmicheskayaLozha.Data;
using KosmicheskayaLozha.Models;
using KosmicheskayaLozha.Services;
using System.Data.Entity;
using System.Linq;
using System.Windows;

namespace KosmicheskayaLozha.Views
{
    public partial class ProductDetailWindow : Window
    {
        private int _productId;

        public ProductDetailWindow(int productId)
        {
            InitializeComponent();
            _productId = productId;
            LoadProduct();
        }

        private void LoadProduct()
        {
            using (var db = new AppDbContext())
            {
                var product = db.Products
                    .Include(p => p.Manufacturer)
                    .Include(p => p.ProductType)
                    .FirstOrDefault(p => p.ProductId == _productId);

                if (product == null) return;

                NameText.Text = product.Name;
                TypeText.Text = $"Тип: {product.ProductType?.Name}";
                ManufacturerText.Text = $"Производитель: {product.Manufacturer?.Name}";
                RatingText.Text = $"Оценка: ⭐ {product.Rating:F1}";
                DescriptionText.Text = product.Description;
                PriceText.Text = $"{product.FinalPrice:F0} руб.";

                if (product.Discount > 0)
                {
                    OldPriceText.Text = $"{product.Price:F0} руб.";
                    DiscountText.Text = $"Скидка: {product.Discount}%";
                }
                else
                {
                    OldPriceText.Visibility = Visibility.Collapsed;
                    DiscountText.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void AddToCartButton_Click(object sender, RoutedEventArgs e)
        {
            if (!AppSession.IsLoggedIn)
            {
                MessageBox.Show("Для добавления в корзину необходимо войти в аккаунт.",
                    "Требуется вход", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            using (var db = new AppDbContext())
            {
                var existing = db.Carts.FirstOrDefault(c =>
                    c.ClientId == AppSession.CurrentUser.UserId &&
                    c.ProductId == _productId);

                if (existing != null)
                    existing.Quantity++;
                else
                    db.Carts.Add(new Cart
                    {
                        ClientId = AppSession.CurrentUser.UserId,
                        ProductId = _productId,
                        Quantity = 1
                    });

                db.SaveChanges();
            }

            MessageBox.Show("Товар добавлен в корзину!", "Готово",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}