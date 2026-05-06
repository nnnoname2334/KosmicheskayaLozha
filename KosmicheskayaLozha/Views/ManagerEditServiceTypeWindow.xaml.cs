using KosmicheskayaLozha.Data;
using KosmicheskayaLozha.Models;
using System;
using System.Windows;

namespace KosmicheskayaLozha.Views
{
    public partial class ManagerEditServiceTypeWindow : Window
    {
        private int _id;

        public ManagerEditServiceTypeWindow(int id)
        {
            InitializeComponent();
            _id = id;
            TitleText.Text = id == 0 ? "Новый тип услуги" : "Редактировать тип услуги";

            if (id > 0)
            {
                using (var db = new AppDbContext())
                {
                    var s = db.ServiceTypes.Find(id);
                    if (s != null)
                    {
                        NameBox.Text = s.Name;
                        PriceBox.Text = s.Price.ToString();
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

            if (!decimal.TryParse(PriceBox.Text, out decimal price) || price < 0)
            {
                MessageBox.Show("Введите корректную цену!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var db = new AppDbContext())
            {
                if (_id == 0)
                    db.ServiceTypes.Add(new ServiceType
                    {
                        Name = NameBox.Text.Trim(),
                        Price = price
                    });
                else
                {
                    var s = db.ServiceTypes.Find(_id);
                    if (s != null)
                    {
                        s.Name = NameBox.Text.Trim();
                        s.Price = price;
                    }
                }
                db.SaveChanges();
            }
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) => this.Close();
    }
}