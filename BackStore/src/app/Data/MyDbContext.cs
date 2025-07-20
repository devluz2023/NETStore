using Microsoft.EntityFrameworkCore;
using MyApi.Models;
namespace MyApi.Data
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Cart> Carts { get; set; }
        // public DbSet<CartProduct> CartProducts { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().OwnsOne(u => u.Name);
            modelBuilder.Entity<User>().OwnsOne(u => u.Address, a =>
            {
                a.OwnsOne(ad => ad.Geolocation);
            });

            // Specify Rating as an owned entity of Product
            modelBuilder.Entity<Product>().OwnsOne(p => p.Rating);
            


 modelBuilder.Entity<Product>()
                .HasOne(p => p.Cart)       // Product has one Cart
                .WithMany(c => c.Products) // Cart has many Products
                .HasForeignKey(p => p.CartId); 
        }

        // Add other DbSets here as needed
    }


}