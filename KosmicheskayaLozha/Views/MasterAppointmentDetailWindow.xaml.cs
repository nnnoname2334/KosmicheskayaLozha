using KosmicheskayaLozha.Data;
using KosmicheskayaLozha.Models;
using System.Data.Entity;
using System.Linq;
using System.Windows;

namespace KosmicheskayaLozha.Views
{
    public partial class MasterAppointmentDetailWindow : Window
    {
        private int _appointmentId;

        public MasterAppointmentDetailWindow(int appointmentId)
        {
            InitializeComponent();
            _appointmentId = appointmentId;
            LoadAppointment();
        }

        private void LoadAppointment()
        {
            using (var db = new AppDbContext())
            {
                var appointment = db.Appointments
                    .Include(a => a.Client)
                    .Include(a => a.ServiceType)
                    .FirstOrDefault(a => a.AppointmentId == _appointmentId);

                if (appointment == null) return;

                DateTimeText.Text = appointment.DateTime.ToString("dd.MM.yyyy HH:mm");
                ClientNameText.Text = appointment.Client?.FullName ?? "—";
                ClientPhoneText.Text = appointment.Client?.Phone ?? "—";
                ServiceText.Text = appointment.ServiceType?.Name ?? "—";
                PriceText.Text = $"{appointment.ServiceType?.Price:F0} руб.";
                PaymentText.Text = appointment.PaymentMethod ?? "—";
                CommentText.Text = string.IsNullOrEmpty(appointment.Comment)
                    ? "Нет комментария" : appointment.Comment;

                if (appointment.Status == "Выполнена")
                    CompleteButton.IsEnabled = false;
            }
        }

        private void CompleteButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Отметить запись как выполненную?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            using (var db = new AppDbContext())
            {
                var appointment = db.Appointments
                    .FirstOrDefault(a => a.AppointmentId == _appointmentId);

                if (appointment == null) return;

                appointment.Status = "Выполнена";
                db.SaveChanges();
            }

            MessageBox.Show("Запись отмечена как выполненная!", "Готово",
                MessageBoxButton.OK, MessageBoxImage.Information);

            this.Close();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}