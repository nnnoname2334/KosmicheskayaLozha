using KosmicheskayaLozha.Data;
using KosmicheskayaLozha.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace KosmicheskayaLozha.Services
{
    public static class AuthService
    {
        public static string HashPassword(string password)
        {
            using (var md5 = MD5.Create())
            {
                var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }

        public static User Login(string login, string password)
        {
            var hash = HashPassword(password);

            MessageBox.Show($"Введён хеш: [{hash}]\nДлина: {hash.Length}");

            using (var db = new AppDbContext())
            {
                // Сначала ищем просто по логину
                var user = db.Users
                    .FirstOrDefault(u => u.Login == login);

                if (user != null)
                    db.Entry(user).Reference(u => u.Role).Load();

                if (user == null)
                {
                    MessageBox.Show("Пользователь по логину не найден!");
                    return null;
                }

                MessageBox.Show($"Найден пользователь: {user.Login}\nХеш в БД: [{user.PasswordHash}]\nДлина: {user.PasswordHash.Length}\nСовпадает: {user.PasswordHash == hash}");

                if (user.PasswordHash != hash) return null;
                if (user.IsFrozen) return null;

                return user;
            }
        }
    }
}