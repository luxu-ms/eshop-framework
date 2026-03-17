using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using eShopLegacy.Models;

namespace eShopLegacy.DAL
{
    public class eShopContext : IdentityDbContext<ApplicationUser>
    {
        public eShopContext(DbContextOptions<eShopContext> options) : base(options) { }

        public DbSet<CatalogItem>  CatalogItems  { get; set; }
        public DbSet<CatalogBrand> CatalogBrands { get; set; }
        public DbSet<CatalogType>  CatalogTypes  { get; set; }
        public DbSet<Basket>       Baskets       { get; set; }
        public DbSet<BasketItem>   BasketItems   { get; set; }
        public DbSet<Order>        Orders        { get; set; }
        public DbSet<OrderItem>    OrderItems    { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CatalogItem>()
                .HasOne(c => c.CatalogBrand)
                .WithMany()
                .HasForeignKey(c => c.CatalogBrandId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<CatalogItem>()
                .HasOne(c => c.CatalogType)
                .WithMany()
                .HasForeignKey(c => c.CatalogTypeId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BasketItem>()
                .HasOne(b => b.Basket)
                .WithMany(b => b.Items)
                .HasForeignKey(b => b.BasketId);

            modelBuilder.Entity<OrderItem>()
                .HasOne(o => o.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(o => o.OrderId);
        }
    }
}
