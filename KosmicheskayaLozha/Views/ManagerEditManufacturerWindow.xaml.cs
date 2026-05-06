using KosmicheskayaLozha.Data;
using KosmicheskayaLozha.Models;
using System.Windows;

namespace KosmicheskayaLozha.Views
{
    public partial class ManagerEditManufacturerWindow : Window
    {
        private int _id;

        public ManagerEditManufacturerWindow(int id)
        {
            InitializeComponent();
            _id = id;
            TitleText.Text = id == 0 ? "Новый производитель" : "Редактировать производителя";

            if (id > 0)
            {
                using (var db = new AppDbContext())
                {
                    var m = db.Manufacturers.Find(id);
                    if (m != null)
                    {
                        NameBox.Text = m.Name;
                        CountryBox.Text = m.Country;
                    }
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
                {
                    db.Manufacturers.Add(new Manufacturer
                    {
                        Name = NameBox.Text.Trim(),
                        Country = CountryBox.Text.Trim()
                    });
                }
                else
                {
                    var m = db.Manufacturers.Find(_id);
                    if (m != null)
                    {
                        m.Name = NameBox.Text.Trim();
                        m.Country = CountryBox.Text.Trim();
                    }
                }
                db.SaveChanges();
            }
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}