using Microsoft.EntityFrameworkCore;
using eShopLegacy.Models;

namespace eShopLegacy.DAL
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(eShopContext context)
        {
            if (await context.CatalogBrands.AnyAsync()) return; // Already seeded

            // Catalog Brands
            var brands = new List<CatalogBrand>
            {
                new CatalogBrand { Brand = "Azure" },
                new CatalogBrand { Brand = ".NET" },
                new CatalogBrand { Brand = "Visual Studio" },
                new CatalogBrand { Brand = "SQL Server" },
                new CatalogBrand { Brand = "Other" }
            };
            context.CatalogBrands.AddRange(brands);
            await context.SaveChangesAsync();

            // Catalog Types
            var types = new List<CatalogType>
            {
                new CatalogType { Type = "Mug" },
                new CatalogType { Type = "T-Shirt" },
                new CatalogType { Type = "Sheet" },
                new CatalogType { Type = "USB Memory Stick" }
            };
            context.CatalogTypes.AddRange(types);
            await context.SaveChangesAsync();

            // Catalog Items
            var items = new List<CatalogItem>
            {
                new CatalogItem { Name = ".NET Bot Black Sweatshirt",   Description = "Black sweatshirt featuring the .NET bot mascot.",       Price = 19.50m,  PictureFileName = "1.png",  PictureUri = "/images/products/1.png",  CatalogTypeId = 2, CatalogBrandId = 2, AvailableStock = 100, RestockThreshold = 10, MaxStockThreshold = 200 },
                new CatalogItem { Name = ".NET Black & White Mug",       Description = "Stylish black and white mug with the .NET logo.",       Price =  8.50m,  PictureFileName = "2.png",  PictureUri = "/images/products/2.png",  CatalogTypeId = 1, CatalogBrandId = 2, AvailableStock =  85, RestockThreshold = 10, MaxStockThreshold = 200 },
                new CatalogItem { Name = "Prism White T-Shirt",          Description = "White T-Shirt featuring the Prism logo.",               Price = 12.00m,  PictureFileName = "3.png",  PictureUri = "/images/products/3.png",  CatalogTypeId = 2, CatalogBrandId = 5, AvailableStock = 100, RestockThreshold = 10, MaxStockThreshold = 200 },
                new CatalogItem { Name = "Prism White & Black Mug",      Description = "Premium white and black muster mug.",                   Price =  8.50m,  PictureFileName = "4.png",  PictureUri = "/images/products/4.png",  CatalogTypeId = 1, CatalogBrandId = 5, AvailableStock =  55, RestockThreshold = 10, MaxStockThreshold = 200 },
                new CatalogItem { Name = "Roslyn Red Sheet",             Description = "Thin sheet with Roslyn compiler logo.",                 Price =  8.50m,  PictureFileName = "5.png",  PictureUri = "/images/products/5.png",  CatalogTypeId = 3, CatalogBrandId = 5, AvailableStock =  70, RestockThreshold = 10, MaxStockThreshold = 200 },
                new CatalogItem { Name = "Roslyn Red T-Shirt",           Description = "Roslyn compiler-themed T-Shirt.",                      Price = 12.00m,  PictureFileName = "6.png",  PictureUri = "/images/products/6.png",  CatalogTypeId = 2, CatalogBrandId = 5, AvailableStock = 100, RestockThreshold = 10, MaxStockThreshold = 200 },
                new CatalogItem { Name = "Kudu Purple Sweatshirt",       Description = "Kudu-branded purple sweatshirt.",                       Price = 19.50m,  PictureFileName = "7.png",  PictureUri = "/images/products/7.png",  CatalogTypeId = 2, CatalogBrandId = 5, AvailableStock =  17, RestockThreshold = 10, MaxStockThreshold = 200 },
                new CatalogItem { Name = "Entity Framework Black Mug",   Description = "Entity Framework-themed black mug.",                   Price =  8.50m,  PictureFileName = "8.png",  PictureUri = "/images/products/8.png",  CatalogTypeId = 1, CatalogBrandId = 2, AvailableStock =  13, RestockThreshold = 10, MaxStockThreshold = 200 },
                new CatalogItem { Name = "Azure Blue T-Shirt",           Description = "Azure cloud-themed T-Shirt in blue.",                  Price = 12.00m,  PictureFileName = "9.png",  PictureUri = "/images/products/9.png",  CatalogTypeId = 2, CatalogBrandId = 1, AvailableStock = 100, RestockThreshold = 10, MaxStockThreshold = 200 },
                new CatalogItem { Name = "Azure Blue Mug",               Description = "Azure-branded blue mug.",                              Price =  8.50m,  PictureFileName = "10.png", PictureUri = "/images/products/10.png", CatalogTypeId = 1, CatalogBrandId = 1, AvailableStock =  25, RestockThreshold = 10, MaxStockThreshold = 200 },
                new CatalogItem { Name = "SQL Server Mug",               Description = "SQL Server database-branded mug.",                     Price =  8.50m,  PictureFileName = "11.png", PictureUri = "/images/products/11.png", CatalogTypeId = 1, CatalogBrandId = 4, AvailableStock =  40, RestockThreshold = 10, MaxStockThreshold = 200 },
                new CatalogItem { Name = "Visual Studio Black & White Mug", Description = "Visual Studio-branded mug.",                       Price =  8.50m,  PictureFileName = "12.png", PictureUri = "/images/products/12.png", CatalogTypeId = 1, CatalogBrandId = 3, AvailableStock = 130, RestockThreshold = 10, MaxStockThreshold = 200 }
            };
            context.CatalogItems.AddRange(items);
            await context.SaveChangesAsync();
        }
    }
}
