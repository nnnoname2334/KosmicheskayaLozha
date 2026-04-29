using System.Collections.Generic;
using System.Windows.Controls;

namespace KosmicheskayaLozha.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Phone { get; set; }
        public string Login { get; set; }
        public string PasswordHash { get; set; }
        public int RoleId { get; set; }
        public bool IsFrozen { get; set; }

        public Role Role { get; set; }
        public ICollection<MasterService> MasterServices { get; set; }
        public ICollection<Appointment> AppointmentsAsClient { get; set; }
        public ICollection<Appointment> AppointmentsAsMaster { get; set; }
        public ICollection<Cart> CartItems { get; set; }
        public ICollection<Order> Orders { get; set; }

        // Удобное свойство — ФИО одной строкой
        public string FullName => $"{LastName} {FirstName} {MiddleName}".Trim();
        // Для отображения на главной странице
        public string ServicesText { get; set; }
    }
}