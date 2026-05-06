using KosmicheskayaLozha.Data;
using KosmicheskayaLozha.Services;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace KosmicheskayaLozha.Views
{
    public partial class AdminWindow : Window
    {
        public AdminWindow()
        {
            InitializeComponent();
            LoadUsers();
        }

        private void LoadUsers()
        {
            using (var db = new AppDbContext())
            {
                var users = db.Users
                    .Include(u => u.Role)
                    .OrderBy(u => u.Role.RoleName)
                    .ThenBy(u => u.LastName)
                    .ToList();

                foreach (var u in users)
                {
                    u.FrozenText = u.IsFrozen ? "Заморожен" : "Активен";
                    u.FreezeButtonText = u.IsFrozen ? "Разморозить" : "Заморозить";
                }

                UsersList.ItemsSource = users;
            }
        }

        private void AddUserButton_Click(object sender, RoutedEventArgs e)
        {
            var w = new AdminEditUserWindow(0);
            w.ShowDialog();
            LoadUsers();
        }

        private void EditUserButton_Click(object sender, RoutedEventArgs e)
        {
            var userId = (int)(sender as Button).Tag;
            var w = new AdminEditUserWindow(userId);
            w.ShowDialog();
            LoadUsers();
        }

        private void FreezeUserButton_Click(object sender, RoutedEventArgs e)
        {
            var userId = (int)(sender as Button).Tag;

            using (var db = new AppDbContext())
            {
                var user = db.Users.Find(userId);
                if (user != null)
                {
                    user.IsFrozen = !user.IsFrozen;
                    db.SaveChanges();
                }
            }
            LoadUsers();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            AppSession.Logout();
            var loginWindow = new MainWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}