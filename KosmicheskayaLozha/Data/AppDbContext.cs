using KosmicheskayaLozha.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Runtime.Remoting.Contexts;

namespace KosmicheskayaLozha.Data
{
    public class AppDbContext : DbContext
    {
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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                "Server=localhost;Database=KosmicheskayaLozha;Trusted_Connection=True;TrustServerCertificate=True;"
            );
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Связь Appointment → Client (может быть null)
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Client)
                .WithMany(u => u.AppointmentsAsClient)
                .HasForeignKey(a => a.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            // Связь Appointment → Master
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Master)
                .WithMany(u => u.AppointmentsAsMaster)
                .HasForeignKey(a => a.MasterId)
                .OnDelete(DeleteBehavior.Restrict);

            // Связь MasterService → Master
            modelBuilder.Entity<MasterService>()
                .HasOne(ms => ms.Master)
                .WithMany(u => u.MasterServices)
                .HasForeignKey(ms => ms.MasterId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}