using Microsoft.EntityFrameworkCore;
using MyApi.Models;
namespace MyApi.Data
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
   
     


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().OwnsOne(u => u.Name);
            modelBuilder.Entity<User>().OwnsOne(u => u.Address, a =>
            {
                a.OwnsOne(ad => ad.Geolocation);
            });

           
            modelBuilder.Entity<Product>().OwnsOne(p => p.Rating);


        }

        
    }


}