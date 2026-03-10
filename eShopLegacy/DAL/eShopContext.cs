using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using eShopLegacy.Models;

namespace eShopLegacy.DAL
{
    public class eShopContext : IdentityDbContext<ApplicationUser>
    {
        public eShopContext()
            : base("eShopContext")
        {
            Configuration.LazyLoadingEnabled = false;
        }

        public static eShopContext Create()
        {
            return new eShopContext();
        }

        public DbSet<CatalogItem>  CatalogItems  { get; set; }
        public DbSet<CatalogBrand> CatalogBrands { get; set; }
        public DbSet<CatalogType>  CatalogTypes  { get; set; }
        public DbSet<Basket>       Baskets       { get; set; }
        public DbSet<BasketItem>   BasketItems   { get; set; }
        public DbSet<Order>        Orders        { get; set; }
        public DbSet<OrderItem>    OrderItems    { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CatalogItem>()
                .HasRequired(c => c.CatalogBrand)
                .WithMany()
                .HasForeignKey(c => c.CatalogBrandId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CatalogItem>()
                .HasRequired(c => c.CatalogType)
                .WithMany()
                .HasForeignKey(c => c.CatalogTypeId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<BasketItem>()
                .HasRequired(b => b.Basket)
                .WithMany(b => b.Items)
                .HasForeignKey(b => b.BasketId);

            modelBuilder.Entity<OrderItem>()
                .HasRequired(o => o.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(o => o.OrderId);
        }
    }
}
