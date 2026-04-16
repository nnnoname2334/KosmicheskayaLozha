using KosmicheskayaLozha.Data;
using System;
using System.Windows;

namespace KosmicheskayaLozha
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                using (var db = new AppDbContext())
                {
                    db.Database.Connection.Open();
                    db.Database.Connection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения к БД:\n{ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }
    }
}