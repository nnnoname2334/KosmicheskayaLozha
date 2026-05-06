using KosmicheskayaLozha.Data;
using KosmicheskayaLozha.Models;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace KosmicheskayaLozha.Views
{
    public partial class ManagerCreateAppointmentWindow : Window
    {
        public ManagerCreateAppointmentWindow()
        {
            InitializeComponent();
            LoadMasters();
        }

        private void LoadMasters()
        {
            using (var db = new AppDbContext())
            {
                var masters = db.Users
                    .Include(u => u.Role)
                    .Where(u => u.Role.RoleName == "Мастер" && !u.IsFrozen)
                    .ToList();
                MasterCombo.ItemsSource = masters;
                MasterCombo.DisplayMemberPath = "FullName";
            }
        }

        private void ClientSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var search = ClientSearchBox.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(search)) return;

            using (var db = new AppDbContext())
            {
                var clients = db.Users
                    .Include(u => u.Role)
                    .Where(u => u.Role.RoleName == "Клиент" &&
                        (u.LastName.ToLower().Contains(search) ||
                         u.FirstName.ToLower().Contains(search) ||
                         u.MiddleName.ToLower().Contains(search) ||
                         u.Phone.Contains(search)))
                    .ToList();

                ClientCombo.ItemsSource = clients;
                ClientCombo.DisplayMemberPath = "FullName";

                if (clients.Count > 0)
                    ClientCombo.IsDropDownOpen = true;
            }
        }

        private void MasterCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(MasterCombo.SelectedItem is User master)) return;

            using (var db = new AppDbContext())
            {
                var slots = db.Appointments
                    .Include(a => a.ServiceType)
                    .Where(a => a.MasterId == master.UserId &&
                                a.Status == "Свободна")
                    .OrderBy(a => a.DateTime)
                    .ToList();

                foreach (var s in slots)
                    s.DateTimeText = $"{s.DateTime:dd.MM.yyyy HH:mm} — {s.ServiceType.Name}";

                SlotCombo.ItemsSource = slots;
                SlotCombo.DisplayMemberPath = "DateTimeText";
            }
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            var client = ClientCombo.SelectedItem as User;
            var slot = SlotCombo.SelectedItem as Appointment;

            if (client == null || slot == null)
            {
                MessageBox.Show("Выберите клиента и свободный слот!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var db = new AppDbContext())
            {
                var appointment = db.Appointments.Find(slot.AppointmentId);
                if (appointment != null)
                {
                    appointment.ClientId = client.UserId;
                    appointment.Status = "Занята";
                    db.SaveChanges();
                }
            }

            MessageBox.Show("Запись создана!", "Готово",
                MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) => this.Close();
    }
}