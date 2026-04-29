using KosmicheskayaLozha.Data;
using KosmicheskayaLozha.Services;
using System.Data.Entity;
using System.Linq;
using System.Windows;

namespace KosmicheskayaLozha.Views
{
    public partial class AccountWindow : Window
    {
        public AccountWindow()
        {
            InitializeComponent();
            LoadUserInfo();
            LoadAppointments();
            LoadOrders();
        }

        private void LoadUserInfo()
        {
            var user = AppSession.CurrentUser;
            UserNameText.Text = user.FullName;
            UserPhoneText.Text = $"Телефон: {user.Phone}";
        }

        private void LoadAppointments()
        {
            using (var db = new AppDbContext())
            {
                var appointments = db.Appointments
                    .Include(a => a.Master)
                    .Include(a => a.ServiceType)
                    .Where(a => a.ClientId == AppSession.CurrentUser.UserId)
                    .OrderByDescending(a => a.DateTime)
                    .ToList();

                foreach (var a in appointments)
                {
                    a.DateTimeFormatted = a.DateTime.ToString("dd.MM.yyyy HH:mm");
                    a.StatusColor = a.Status == "Выполнена" ? "#4CAF50" :
                                    a.Status == "Отменена" ? "#F44336" : "#7B2D8B";
                }

                AppointmentsList.ItemsSource = appointments;
            }
        }

        private void LoadOrders()
        {
            using (var db = new AppDbContext())
            {
                var orders = db.Orders
                    .Include(o => o.OrderItems.Select(oi => oi.Product))
                    .Where(o => o.ClientId == AppSession.CurrentUser.UserId)
                    .OrderByDescending(o => o.OrderDate)
                    .ToList();

                foreach (var o in orders)
                {
                    o.OrderDateFormatted = $"Заказ от {o.OrderDate:dd.MM.yyyy}";
                    o.DeliveryDateFormatted = $"Доставка: {o.DeliveryDate:dd.MM.yyyy}";
                    var total = o.OrderItems.Sum(oi => oi.PriceAtOrder * oi.Quantity);
                    o.TotalText = $"Итого: {total:F0} руб.";
                }

                OrdersList.ItemsSource = orders;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            AppSession.Logout();
            var loginWindow = new MainWindow();
            loginWindow.Show();

            // Закрываем все окна кроме нового
            foreach (Window w in Application.Current.Windows)
            {
                if (w != loginWindow) w.Close();
            }
        }
    }
}