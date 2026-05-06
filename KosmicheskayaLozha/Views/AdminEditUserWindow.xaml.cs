using System.Windows;

namespace KosmicheskayaLozha.Views
{
    public partial class AdminEditUserWindow : Window
    {
        private int _id;

        public AdminEditUserWindow(int id)
        {
            InitializeComponent();
            _id = id;
        }
    }
}