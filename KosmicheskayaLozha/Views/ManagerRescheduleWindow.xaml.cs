using KosmicheskayaLozha.Data;
using KosmicheskayaLozha.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;

namespace KosmicheskayaLozha.Views
{
    public partial class ManagerRescheduleWindow : Window
    {
        private int _appointmentId;

        public ManagerRescheduleWindow(int appointmentId)
        {
            InitializeComponent();
            _appointmentId = appointmentId;

            using (var db = new AppDbContext())
            {
                var a = db.Appointments
                    .Include(x => x.ServiceType)
                    .Include(x => x.Master)
                    .FirstOrDefault(x => x.AppointmentId == appointmentId);

                if (a != null)
                {
                    CurrentInfoText.Text =
                        $"Текущая запись: {a.DateTime:dd.MM.yyyy HH:mm}\n" +
                        $"Мастер: {a.Master.FullName}\n" +
                        $"Услуга: {a.ServiceType.Name}";

                    NewDatePicker.SelectedDate = a.DateTime.Date;
                    NewTimeBox.Text = a.DateTime.ToString("HH:mm");
                }
            }
        }

        private void RescheduleButton_Click(object sender, RoutedEventArgs e)
        {
            if (NewDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Выберите дату!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!TimeSpan.TryParse(NewTimeBox.Text, out TimeSpan time))
            {
                MessageBox.Show("Введите время в формате HH:mm!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newDateTime = NewDatePicker.SelectedDate.Value.Date + time;

            using (var db = new AppDbContext())
            {
                var a = db.Appointments.Find(_appointmentId);
                if (a != null)
                {
                    a.DateTime = newDateTime;
                    db.SaveChanges();
                }
            }

            MessageBox.Show("Запись перенесена!", "Готово",
                MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) => this.Close();
    }
}