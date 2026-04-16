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

            // Проверяем подключение к БД
            try
            {
                using var db = new AppDbContext();
                db.Database.CanConnect();
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