using KosmicheskayaLozha.Data;
using KosmicheskayaLozha.Models;
using System.Linq;
using System.Windows;

namespace KosmicheskayaLozha.Views
{
    public partial class ManagerEditProductWindow : Window
    {
        private int _id;

        public ManagerEditProductWindow(int id)
        {
            InitializeComponent();
            _id = id;
            TitleText.Text = id == 0 ? "Новый товар" : "Редактировать товар";
            LoadCombos();

            if (id > 0)
            {
                using (var db = new AppDbContext())
                {
                    var p = db.Products.Find(id);
                    if (p != null)
                    {
                        NameBox.Text = p.Name;
                        PriceBox.Text = p.Price.ToString();
                        DiscountBox.Text = p.Discount.ToString();
                        DescriptionBox.Text = p.Description;

                        foreach (Manufacturer item in ManufacturerCombo.Items)
                            if (item.ManufacturerId == p.ManufacturerId)
                            { ManufacturerCombo.SelectedItem = item; break; }

                        foreach (ProductType item in ProductTypeCombo.Items)
                            if (item.ProductTypeId == p.ProductTypeId)
                            { ProductTypeCombo.SelectedItem = item; break; }
                    }
                }
            }
        }

        private void LoadCombos()
        {
            using (var db = new AppDbContext())
            {
                ManufacturerCombo.ItemsSource = db.Manufacturers.ToList();
                ManufacturerCombo.DisplayMemberPath = "Name";

                ProductTypeCombo.ItemsSource = db.ProductTypes.ToList();
                ProductTypeCombo.DisplayMemberPath = "Name";
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                MessageBox.Show("Введите название!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(PriceBox.Text, out decimal price) || price < 0)
            {
                MessageBox.Show("Введите корректную цену!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(DiscountBox.Text, out int discount))
                discount = 0;

            var manufacturer = ManufacturerCombo.SelectedItem as Manufacturer;
            var productType = ProductTypeCombo.SelectedItem as ProductType;

            if (manufacturer == null || productType == null)
            {
                MessageBox.Show("Выберите производителя и тип товара!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var db = new AppDbContext())
            {
                if (_id == 0)
                {
                    db.Products.Add(new Product
                    {
                        Name = NameBox.Text.Trim(),
                        Price = price,
                        Discount = discount,
                        Description = DescriptionBox.Text.Trim(),
                        ManufacturerId = manufacturer.ManufacturerId,
                        ProductTypeId = productType.ProductTypeId,
                        Rating = 0,
                        IsFrozen = false
                    });
                }
                else
                {
                    var p = db.Products.Find(_id);
                    if (p != null)
                    {
                        p.Name = NameBox.Text.Trim();
                        p.Price = price;
                        p.Discount = discount;
                        p.Description = DescriptionBox.Text.Trim();
                        p.ManufacturerId = manufacturer.ManufacturerId;
                        p.ProductTypeId = productType.ProductTypeId;
                    }
                }
                db.SaveChanges();
            }
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) => this.Close();
    }
}