using Microsoft.EntityFrameworkCore;
using MyApi.Models;
namespace MyApi.Data
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Specify Rating as an owned entity of Product
            modelBuilder.Entity<Product>().OwnsOne(p => p.Rating);
        }

        // Add other DbSets here as needed
    }
    
    
}