using KosmicheskayaLozha.Models;
using System.Data.Entity;

namespace KosmicheskayaLozha.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext()
            : base("name=KosmicheskayaLozha")
        {
        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ServiceType> ServiceTypes { get; set; }
        public DbSet<MasterService> MasterServices { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Связь Appointment → Client
            modelBuilder.Entity<Appointment>()
                .HasOptional(a => a.Client)
                .WithMany(u => u.AppointmentsAsClient)
                .HasForeignKey(a => a.ClientId)
                .WillCascadeOnDelete(false);

            // Связь Appointment → Master
            modelBuilder.Entity<Appointment>()
                .HasRequired(a => a.Master)
                .WithMany(u => u.AppointmentsAsMaster)
                .HasForeignKey(a => a.MasterId)
                .WillCascadeOnDelete(false);

            // Связь MasterService → Master
            modelBuilder.Entity<MasterService>()
                .HasRequired(ms => ms.Master)
                .WithMany(u => u.MasterServices)
                .HasForeignKey(ms => ms.MasterId)
                .WillCascadeOnDelete(false);
        }
    }
}