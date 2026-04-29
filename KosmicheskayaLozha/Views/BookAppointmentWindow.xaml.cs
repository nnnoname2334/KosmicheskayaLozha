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
        }
    }
}