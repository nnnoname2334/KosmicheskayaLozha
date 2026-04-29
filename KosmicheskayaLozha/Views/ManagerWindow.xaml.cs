using KosmicheskayaLozha.Data;
using KosmicheskayaLozha.Models;
using KosmicheskayaLozha.Services;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace KosmicheskayaLozha.Views
{
    public partial class ManagerWindow : Window
    {
        public ManagerWindow()
        {
            InitializeComponent();
            LoadAll();
        }

        private void LoadAll()
        {
            LoadAppointments();
            LoadOrders();
            LoadProducts();
            LoadManufacturers();
            LoadProductTypes();
            LoadServiceTypes();
        }

        private void LoadAppointments()
        {
            using (var db = new AppDbContext())
            {
                var appointments = db.Appointments
                    .Include(a => a.Client)
                    .Include(a => a.Master)
                    .Include(a => a.ServiceType)
                    .OrderByDescending(a => a.DateTime)
                    .ToList();

                foreach (var a in appointments)
                    a.DateTimeFormatted = a.DateTime.ToString("dd.MM.yyyy HH:mm");

                AppointmentsList.ItemsSource = appointments;
            }
        }

        private void LoadOrders()
        {
            using (var db = new AppDbContext())
            {
                var orders = db.Orders
                    .Include(o => o.Client)
                    .OrderByDescending(o => o.OrderDate)
                    .ToList();

                foreach (var o in orders)
                {
                    o.OrderDateFormatted = o.OrderDate.ToString("dd.MM.yyyy");
                    o.DeliveryDateFormatted = o.DeliveryDate.ToString("dd.MM.yyyy");
                }

                OrdersList.ItemsSource = orders;
            }
        }

        private void LoadProducts()
        {
            using (var db = new AppDbContext())
            {
                var products = db.Products
                    .Include(p => p.Manufacturer)
                    .Include(p => p.ProductType)
                    .ToList();

                foreach (var p in products)
                {
                    p.PriceText = $"{p.Price:F0} руб.";
                    p.DiscountText = p.Discount > 0 ? $"-{p.Discount}%" : "";
                    p.FrozenText = p.IsFrozen ? "Заморожен" : "Активен";
                    p.FreezeButtonText = p.IsFrozen ? "Разморозить" : "Заморозить";
                }

                ProductsList.ItemsSource = products;
            }
        }

        private void LoadManufacturers()
        {
            using (var db = new AppDbContext())
            {
                ManufacturersList.ItemsSource = db.Manufacturers.ToList();
            }
        }

        private void LoadProductTypes()
        {
            using (var db = new AppDbContext())
            {
                ProductTypesList.ItemsSource = db.ProductTypes.ToList();
            }
        }

        private void LoadServiceTypes()
        {
            using (var db = new AppDbContext())
            {
                var types = db.ServiceTypes.ToList();
                foreach (var t in types)
                    t.PriceText = $"{t.Price:F0} руб.";
                ServiceTypesList.ItemsSource = types;
            }
        }

        // Записи
        private void CreateAppointmentButton_Click(object sender, RoutedEventArgs e)
        {
            var w = new ManagerCreateAppointmentWindow();
            w.ShowDialog();
            LoadAppointments();
        }

        private void RescheduleButton_Click(object sender, RoutedEventArgs e)
        {
            if (AppointmentsList.SelectedItem is Appointment a)
            {
                var w = new ManagerRescheduleWindow(a.AppointmentId);
                w.ShowDialog();
                LoadAppointments();
            }
            else
                MessageBox.Show("Выберите запись!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void CancelAppointmentButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(AppointmentsList.SelectedItem is Appointment a))
            {
                MessageBox.Show("Выберите запись!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show("Отменить запись?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            using (var db = new AppDbContext())
            {
                var appointment = db.Appointments
                    .FirstOrDefault(x => x.AppointmentId == a.AppointmentId);
                if (appointment != null)
                {
                    appointment.Status = "Отменена";
                    db.SaveChanges();
                }
            }
            LoadAppointments();
        }

        // Заказы
        private void CompleteOrderButton_Click(object sender, RoutedEventArgs e)
        {
            var orderId = (int)(sender as Button).Tag;

            var result = MessageBox.Show("Отметить заказ как выданный?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            using (var db = new AppDbContext())
            {
                var order = db.Orders.FirstOrDefault(o => o.OrderId == orderId);
                if (order != null)
                {
                    order.Status = "Выдан";
                    db.SaveChanges();
                }
            }
            LoadOrders();
        }

        // Товары
        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            var w = new ManagerEditProductWindow(0);
            w.ShowDialog();
            LoadProducts();
        }

        private void EditProductButton_Click(object sender, RoutedEventArgs e)
        {
            var productId = (int)(sender as Button).Tag;
            var w = new ManagerEditProductWindow(productId);
            w.ShowDialog();
            LoadProducts();
        }

        private void FreezeProductButton_Click(object sender, RoutedEventArgs e)
        {
            var productId = (int)(sender as Button).Tag;

            using (var db = new AppDbContext())
            {
                var product = db.Products.FirstOrDefault(p => p.ProductId == productId);
                if (product != null)
                {
                    product.IsFrozen = !product.IsFrozen;
                    db.SaveChanges();
                }
            }
            LoadProducts();
        }

        // Производители
        private void AddManufacturerButton_Click(object sender, RoutedEventArgs e)
        {
            var w = new ManagerEditManufacturerWindow(0);
            w.ShowDialog();
            LoadManufacturers();
        }

        private void EditManufacturerButton_Click(object sender, RoutedEventArgs e)
        {
            var id = (int)(sender as Button).Tag;
            var w = new ManagerEditManufacturerWindow(id);
            w.ShowDialog();
            LoadManufacturers();
        }

        // Типы товаров
        private void AddProductTypeButton_Click(object sender, RoutedEventArgs e)
        {
            var w = new ManagerEditProductTypeWindow(0);
            w.ShowDialog();
            LoadProductTypes();
        }

        private void EditProductTypeButton_Click(object sender, RoutedEventArgs e)
        {
            var id = (int)(sender as Button).Tag;
            var w = new ManagerEditProductTypeWindow(id);
            w.ShowDialog();
            LoadProductTypes();
        }

        // Типы услуг
        private void AddServiceTypeButton_Click(object sender, RoutedEventArgs e)
        {
            var w = new ManagerEditServiceTypeWindow(0);
            w.ShowDialog();
            LoadServiceTypes();
        }

        private void EditServiceTypeButton_Click(object sender, RoutedEventArgs e)
        {
            var id = (int)(sender as Button).Tag;
            var w = new ManagerEditServiceTypeWindow(id);
            w.ShowDialog();
            LoadServiceTypes();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            AppSession.Logout();
            var loginWindow = new MainWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}