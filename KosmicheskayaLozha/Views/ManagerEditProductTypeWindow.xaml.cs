using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KosmicheskayaLozha.Views
{
    /// <summary>
    /// Логика взаимодействия для ManagerEditProductTypeWindow.xaml
    /// </summary>
    public partial class ManagerEditProductTypeWindow : Window
    {
        private int _id;
        public ManagerEditProductTypeWindow(int id) { InitializeComponent(); _id = id; }
    }
}
