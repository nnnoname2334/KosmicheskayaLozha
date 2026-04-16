using KosmicheskayaLozha.Services;
using KosmicheskayaLozha.Views;
using System.Windows;

namespace KosmicheskayaLozha
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var login = LoginBox.Text.Trim();
            var password = PasswordBox.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                ErrorText.Text = "Введите логин и пароль";
                return;
            }

            var user = AuthService.Login(login, password);

            if (user == null)
            {
                ErrorText.Text = "Неверный логин или пароль";
                return;
            }

            // Сохраняем пользователя в сессию
            AppSession.CurrentUser = user;

            // Открываем нужную страницу в зависимости от роли
            Window nextWindow = null;

            switch (user.Role.RoleName)
            {
                case "Клиент":
                    nextWindow = new MainMenuWindow();
                    break;
                case "Мастер":
                    nextWindow = new MasterWindow();
                    break;
                case "Менеджер":
                    nextWindow = new ManagerWindow();
                    break;
                case "Администратор":
                    nextWindow = new AdminWindow();
                    break;
            }

            if (nextWindow != null)
            {
                nextWindow.Show();
                this.Close();
            }
        }
    }
}