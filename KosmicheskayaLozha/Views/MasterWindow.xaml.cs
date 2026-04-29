using KosmicheskayaLozha.Data;
using KosmicheskayaLozha.Models;
using KosmicheskayaLozha.Services;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace KosmicheskayaLozha.Views
{
    public partial class MasterWindow : Window
    {
        private bool _loadingServices = false;

        public MasterWindow()
        {
            InitializeComponent();
            MasterNameText.Text = $"🌸 {AppSession.CurrentUser.FullName}";
            LoadServices();
            LoadAppointments();
        }

        private void LoadServices()
        {
            _loadingServices = true;

            using (var db = new AppDbContext())
            {
                var allServices = db.ServiceTypes.ToList();
                var myServiceIds = db.MasterServices
                    .Where(ms => ms.MasterId == AppSession.CurrentUser.UserId)
                    .Select(ms => ms.ServiceTypeId)
                    .ToList();

                var serviceItems = allServices.Select(s => new ServiceCheckItem
                {
                    ServiceTypeId = s.ServiceTypeId,
                    Name = s.Name,
                    IsSelected = myServiceIds.Contains(s.ServiceTypeId)
                }).ToList();

                ServicesList.ItemsSource = serviceItems;
            }

            _loadingServices = false;
        }

        private void LoadAppointments()
        {
            using (var db = new AppDbContext())
            {
                var appointments = db.Appointments
                    .Include(a => a.Client)
                    .Include(a => a.ServiceType)
                    .Where(a => a.MasterId == AppSession.CurrentUser.UserId
                                && a.Status == "Занята")
                    .OrderBy(a => a.DateTime)
                    .ToList();

                foreach (var a in appointments)
                {
                    a.DateTimeFormatted = a.DateTime.ToString("dd.MM.yyyy HH:mm");
                    a.StatusColor = "#7B2D8B";
                }

                AppointmentsList.ItemsSource = appointments;
            }
        }

        private void ServiceCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (_loadingServices) return;

            var checkBox = sender as CheckBox;
            var serviceTypeId = (int)checkBox.Tag;
            var isChecked = checkBox.IsChecked == true;

            using (var db = new AppDbContext())
            {
                var existing = db.MasterServices.FirstOrDefault(ms =>
                    ms.MasterId == AppSession.CurrentUser.UserId &&
                    ms.ServiceTypeId == serviceTypeId);

                if (isChecked && existing == null)
                {
                    db.MasterServices.Add(new MasterService
                    {
                        MasterId = AppSession.CurrentUser.UserId,
                        ServiceTypeId = serviceTypeId
                    });
                }
                else if (!isChecked && existing != null)
                {
                    db.MasterServices.Remove(existing);
                }

                db.SaveChanges();
            }
        }

        private void AppointmentsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AppointmentsList.SelectedItem is Appointment appointment)
            {
                var detailWindow = new MasterAppointmentDetailWindow(appointment.AppointmentId);
                detailWindow.ShowDialog();
                LoadAppointments();
                AppointmentsList.SelectedItem = null;
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            AppSession.Logout();
            var loginWindow = new MainWindow();
            loginWindow.Show();
            this.Close();
        }
    }

    // Вспомогательный класс для чекбоксов услуг
    public class ServiceCheckItem
    {
        public int ServiceTypeId { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }
}