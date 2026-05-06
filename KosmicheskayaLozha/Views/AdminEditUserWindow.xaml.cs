using KosmicheskayaLozha.Data;
using KosmicheskayaLozha.Models;
using KosmicheskayaLozha.Services;
using System.Linq;
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
            TitleText.Text = id == 0 ? "Новый пользователь" : "Редактировать пользователя";
            LoadRoles();

            if (id > 0)
            {
                using (var db = new AppDbContext())
                {
                    var user = db.Users.Find(id);
                    if (user != null)
                    {
                        LastNameBox.Text = user.LastName;
                        FirstNameBox.Text = user.FirstName;
                        MiddleNameBox.Text = user.MiddleName;
                        PhoneBox.Text = user.Phone;
                        LoginBox.Text = user.Login;

                        foreach (Role item in RoleCombo.Items)
                            if (item.RoleId == user.RoleId)
                            {
                                RoleCombo.SelectedItem = item;
                                break;
                            }
                    }
                }
            }
        }

        private void LoadRoles()
        {
            using (var db = new AppDbContext())
            {
                RoleCombo.ItemsSource = db.Roles.ToList();
                RoleCombo.DisplayMemberPath = "RoleName";
                RoleCombo.SelectedIndex = 0;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LastNameBox.Text) ||
                string.IsNullOrWhiteSpace(FirstNameBox.Text) ||
                string.IsNullOrWhiteSpace(LoginBox.Text))
            {
                MessageBox.Show("Заполните обязательные поля (Фамилия, Имя, Логин)!",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedRole = RoleCombo.SelectedItem as Role;
            if (selectedRole == null)
            {
                MessageBox.Show("Выберите роль!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var db = new AppDbContext())
            {
                if (_id == 0)
                {
                    // Новый пользователь — пароль обязателен
                    if (string.IsNullOrWhiteSpace(PasswordBox.Password))
                    {
                        MessageBox.Show("Введите пароль для нового пользователя!",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Проверяем уникальность логина
                    if (db.Users.Any(u => u.Login == LoginBox.Text.Trim()))
                    {
                        MessageBox.Show("Пользователь с таким логином уже существует!",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    db.Users.Add(new User
                    {
                        LastName = LastNameBox.Text.Trim(),
                        FirstName = FirstNameBox.Text.Trim(),
                        MiddleName = MiddleNameBox.Text.Trim(),
                        Phone = PhoneBox.Text.Trim(),
                        Login = LoginBox.Text.Trim(),
                        PasswordHash = AuthService.HashPassword(PasswordBox.Password),
                        RoleId = selectedRole.RoleId,
                        IsFrozen = false
                    });
                }
                else
                {
                    var user = db.Users.Find(_id);
                    if (user != null)
                    {
                        user.LastName = LastNameBox.Text.Trim();
                        user.FirstName = FirstNameBox.Text.Trim();
                        user.MiddleName = MiddleNameBox.Text.Trim();
                        user.Phone = PhoneBox.Text.Trim();
                        user.Login = LoginBox.Text.Trim();
                        user.RoleId = selectedRole.RoleId;

                        // Меняем пароль только если введён
                        if (!string.IsNullOrWhiteSpace(PasswordBox.Password))
                            user.PasswordHash = AuthService.HashPassword(PasswordBox.Password);
                    }
                }

                db.SaveChanges();
            }

            MessageBox.Show("Сохранено!", "Готово",
                MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) => this.Close();
    }
}