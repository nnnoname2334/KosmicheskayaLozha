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
            modelBuilder.Entity<User>().Ignore(u => u.FrozenText);
            modelBuilder.Entity<User>().Ignore(u => u.FreezeButtonText);
            modelBuilder.Entity<User>().Ignore(u => u.ServicesText);

            modelBuilder.Entity<Appointment>().Ignore(a => a.DateTimeText);
            modelBuilder.Entity<Appointment>().Ignore(a => a.PriceText);
            modelBuilder.Entity<Appointment>().Ignore(a => a.DateTimeFormatted);
            modelBuilder.Entity<Appointment>().Ignore(a => a.StatusColor);
            modelBuilder.Entity<Order>().Ignore(o => o.OrderDateFormatted);
            modelBuilder.Entity<Order>().Ignore(o => o.DeliveryDateFormatted);
            modelBuilder.Entity<Order>().Ignore(o => o.TotalText);
            modelBuilder.Entity<Cart>().ToTable("Cart");
            modelBuilder.Entity<Order>().ToTable("Orders");
            modelBuilder.Entity<OrderItem>().ToTable("OrderItems");
            modelBuilder.Entity<Product>().Ignore(p => p.PriceText);
            modelBuilder.Entity<Product>().Ignore(p => p.DiscountText);
            modelBuilder.Entity<Product>().Ignore(p => p.FrozenText);
            modelBuilder.Entity<Product>().Ignore(p => p.FreezeButtonText);
            modelBuilder.Entity<ServiceType>().Ignore(s => s.PriceText);
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

            modelBuilder.Entity<Order>()
                .HasRequired(o => o.Client)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.ClientId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Cart>()
                .HasRequired(c => c.Client)
                .WithMany(u => u.CartItems)
                .HasForeignKey(c => c.ClientId)
                .WillCascadeOnDelete(false);
        }
    }
}