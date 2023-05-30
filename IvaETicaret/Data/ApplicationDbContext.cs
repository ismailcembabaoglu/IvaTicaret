using IvaETicaret.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IvaETicaret.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<IvaETicaret.Models.Category> Categories { get; set; }
        public DbSet<IvaETicaret.Models.Department> Departments { get; set; }
        public DbSet<IvaETicaret.Models.Product> Products { get; set; }
        public DbSet<IvaETicaret.Models.Store> Stores { get; set; }
        public DbSet<IvaETicaret.Models.StoreAdress> StoreAdresses { get; set; }
        public DbSet<IvaETicaret.Models.ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<IvaETicaret.Models.ShoppingKart> ShoppingKarts { get; set; }
        public DbSet<IvaETicaret.Models.OrderHeader> OrderHeaders { get; set; }
        public DbSet<IvaETicaret.Models.OrderDetail> OrderDetails { get; set; }
        public DbSet<IvaETicaret.Models.County> Counties { get; set; }
        public DbSet<IvaETicaret.Models.City> Cities { get; set; }
        public DbSet<IvaETicaret.Models.District> Districts { get; set; }
        public DbSet<IvaETicaret.Models.Adress> Adress { get; set; }
        public DbSet<IvaETicaret.Models.OdemeTur> OdemeTurleri { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<OrderHeader>(b =>
            {
                b.HasOne(c => c.Adress).WithMany(c => c.OrderHeaders).OnDelete(DeleteBehavior.NoAction);
                b.HasOne(c => c.Store).WithMany(c => c.OrderHeaders).OnDelete(DeleteBehavior.NoAction);
            });

        }
    }
}