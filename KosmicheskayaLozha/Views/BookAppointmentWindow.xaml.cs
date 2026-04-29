using KosmicheskayaLozha.Data;
using KosmicheskayaLozha.Services;
using System.Data.Entity;
using System.Linq;
using System.Windows;

namespace KosmicheskayaLozha.Views
{
    public partial class BookAppointmentWindow : Window
    {
        private int _appointmentId;

        public BookAppointmentWindow(int appointmentId)
        {
            InitializeComponent();
            _appointmentId = appointmentId;
            LoadAppointmentInfo();
        }

        private void LoadAppointmentInfo()
        {
            using (var db = new AppDbContext())
            {
                var appointment = db.Appointments
                    .Include(a => a.Master)
                    .Include(a => a.ServiceType)
                    .FirstOrDefault(a => a.AppointmentId == _appointmentId);

                if (appointment == null) return;

                ServiceText.Text = $"Услуга: {appointment.ServiceType.Name}";
                MasterText.Text = $"Мастер: {appointment.Master.FullName}";
                DateTimeText.Text = $"Дата и время: {appointment.DateTime:dd.MM.yyyy HH:mm}";
                PriceText.Text = $"Цена: {appointment.ServiceType.Price:F0} руб.";
            }
        }

        private void BookButton_Click(object sender, RoutedEventArgs e)
        {
            var payment = (PaymentCombo.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString();

            var result = MessageBox.Show(
                "Вы подтверждаете запись на сеанс?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            using (var db = new AppDbContext())
            {
                var appointment = db.Appointments
                    .FirstOrDefault(a => a.AppointmentId == _appointmentId);

                if (appointment == null) return;

                if (appointment.Status != "Свободна")
                {
                    MessageBox.Show("Эта запись уже занята!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                appointment.ClientId = AppSession.CurrentUser.UserId;
                appointment.Status = "Занята";
                appointment.PaymentMethod = payment;
                appointment.Comment = CommentBox.Text.Trim();

                db.SaveChanges();
            }

            MessageBox.Show("Вы успешно записаны!", "Готово",
                MessageBoxButton.OK, MessageBoxImage.Information);

            this.Close();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}