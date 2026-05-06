using KosmicheskayaLozha.Data;
using KosmicheskayaLozha.Models;
using System.Windows;

namespace KosmicheskayaLozha.Views
{
    public partial class ManagerEditProductTypeWindow : Window
    {
        private int _id;

        public ManagerEditProductTypeWindow(int id)
        {
            InitializeComponent();
            _id = id;
            TitleText.Text = id == 0 ? "Новый тип товара" : "Редактировать тип товара";

            if (id > 0)
            {
                using (var db = new AppDbContext())
                {
                    var t = db.ProductTypes.Find(id);
                    if (t != null) NameBox.Text = t.Name;
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                MessageBox.Show("Введите название!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var db = new AppDbContext())
            {
                if (_id == 0)
                    db.ProductTypes.Add(new ProductType { Name = NameBox.Text.Trim() });
                else
                {
                    var t = db.ProductTypes.Find(_id);
                    if (t != null) t.Name = NameBox.Text.Trim();
                }
                db.SaveChanges();
            }
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) => this.Close();
    }
}