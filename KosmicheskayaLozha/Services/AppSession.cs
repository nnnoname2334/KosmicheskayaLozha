using KosmicheskayaLozha.Models;

namespace KosmicheskayaLozha.Services
{
    public static class AppSession
    {
        public static User CurrentUser { get; set; }

        public static bool IsLoggedIn => CurrentUser != null;

        public static bool IsClient => CurrentUser?.Role?.RoleName == "Клиент";
        public static bool IsMaster => CurrentUser?.Role?.RoleName == "Мастер";
        public static bool IsManager => CurrentUser?.Role?.RoleName == "Менеджер";
        public static bool IsAdmin => CurrentUser?.Role?.RoleName == "Администратор";

        public static void Logout()
        {
            CurrentUser = null;
        }
    }
}