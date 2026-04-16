using KosmicheskayaLozha.Data;
using KosmicheskayaLozha.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

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

            using (var db = new AppDbContext())
            {
                var user = db.Users
                    .Include(u => u.Role)
                    .FirstOrDefault(u => u.Login == login && u.PasswordHash == hash);

                if (user == null) return null;
                if (user.IsFrozen) return null;

                return user;
            }
        }
    }
}