using KosmicheskayaLozha.Data;
using KosmicheskayaLozha.Models;
using KosmicheskayaLozha.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace KosmicheskayaLozha.Views
{
    public partial class MainMenuWindow : Window
    {
        private List<User> _allMasters;
        private List<Appointment> _allAppointments;

        public MainMenuWindow()
        {
            InitializeComponent();
            UpdateUI();
            LoadFilters();
            LoadMasters();
        }

        // Показываем/скрываем кнопки в зависимости от входа
        private void UpdateUI()
        {
            if (AppSession.IsLoggedIn)
            {
                LoginButton.Visibility = Visibility.Collapsed;
                AccountButton.Visibility = Visibility.Visible;
                AccountButton.Content = $"👤 {AppSession.CurrentUser.FirstName}";
            }
            else
            {
                LoginButton.Visibility = Visibility.Visible;
                AccountButton.Visibility = Visibility.Collapsed;
            }
        }

        private void LoadFilters()
        {
            using (var db = new AppDbContext())
            {
                // Типы услуг
                var serviceTypes = db.ServiceTypes.ToList();
                serviceTypes.Insert(0, new ServiceType { ServiceTypeId = 0, Name = "Все услуги" });
                ServiceTypeCombo.ItemsSource = serviceTypes;
                ServiceTypeCombo.DisplayMemberPath = "Name";
                ServiceTypeCombo.SelectedIndex = 0;

                // Мастера
                var masters = db.Users
                    .Include(u => u.Role)
                    .Where(u => u.Role.RoleName == "Мастер" && !u.IsFrozen)
                    .ToList();
                _allMasters = masters;

                var mastersForFilter = new List<User> { new User { UserId = 0, LastName = "Все", FirstName = "мастера" } };
                mastersForFilter.AddRange(masters);
                MasterCombo.ItemsSource = mastersForFilter;
                MasterCombo.DisplayMemberPath = "FullName";
                MasterCombo.SelectedIndex = 0;
            }
        }

        private void LoadMasters()
        {
            using (var db = new AppDbContext())
            {
                var masters = db.Users
                    .Include(u => u.Role)
                    .Include(u => u.MasterServices.Select(ms => ms.ServiceType))
                    .Where(u => u.Role.RoleName == "Мастер" && !u.IsFrozen)
                    .ToList();

                // Добавляем текст услуг для отображения
                foreach (var master in masters)
                {
                    var services = master.MasterServices
                        .Select(ms => ms.ServiceType.Name)
                        .ToList();
                    master.ServicesText = string.Join(", ", services);
                }

                MastersList.ItemsSource = masters;
            }
        }

        private void LoadAppointments(int? masterId = null, int? serviceTypeId = null)
        {
            using (var db = new AppDbContext())
            {
                var query = db.Appointments
                    .Include(a => a.Master)
                    .Include(a => a.ServiceType)
                    .Where(a => a.Status == "Свободна" && a.DateTime >= DateTime.Now);

                if (masterId.HasValue && masterId > 0)
                    query = query.Where(a => a.MasterId == masterId.Value);

                if (serviceTypeId.HasValue && serviceTypeId > 0)
                    query = query.Where(a => a.ServiceTypeId == serviceTypeId.Value);

                var appointments = query.OrderBy(a => a.DateTime).ToList();

                // Добавляем форматированные поля
                foreach (var a in appointments)
                {
                    a.DateTimeText = a.DateTime.ToString("dd.MM.yyyy HH:mm");
                    a.PriceText = $"Цена: {a.ServiceType.Price:F0} руб.";
                }

                AppointmentsList.ItemsSource = appointments;
            }
        }

        private void ServiceTypeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void MasterCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void MastersList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MastersList.SelectedItem is User master)
            {
                MasterCombo.SelectedItem = MasterCombo.Items
                    .Cast<User>()
                    .FirstOrDefault(u => u.UserId == master.UserId);
                ApplyFilters();
            }
        }

        private void ApplyFilters()
        {
            var selectedService = ServiceTypeCombo.SelectedItem as ServiceType;
            var selectedMaster = MasterCombo.SelectedItem as User;

            int? serviceId = selectedService?.ServiceTypeId > 0 ? selectedService.ServiceTypeId : (int?)null;
            int? masterId = selectedMaster?.UserId > 0 ? selectedMaster.UserId : (int?)null;

            LoadAppointments(masterId, serviceId);
        }

        private void BookButton_Click(object sender, RoutedEventArgs e)
        {
            if (!AppSession.IsLoggedIn)
            {
                MessageBox.Show("Для записи необходимо войти в аккаунт.",
                    "Требуется вход", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var button = sender as Button;
            var appointmentId = (int)button.Tag;

            // Подтверждение записи
            var result = MessageBox.Show("Вы хотите записаться на этот сеанс?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var bookWindow = new BookAppointmentWindow(appointmentId);
                bookWindow.ShowDialog();
                ApplyFilters(); // Обновляем список после записи
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new MainWindow();
            loginWindow.Show();
            this.Close();
        }

        private void AccountButton_Click(object sender, RoutedEventArgs e)
        {
            var accountWindow = new AccountWindow();
            accountWindow.ShowDialog();
        }

        private void ProductsButton_Click(object sender, RoutedEventArgs e)
        {
            var productsWindow = new ProductsWindow();
            productsWindow.ShowDialog();
        }
    }
}