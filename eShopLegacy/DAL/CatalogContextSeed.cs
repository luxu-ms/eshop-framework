using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eShopLegacy.Models;
using Microsoft.EntityFrameworkCore;

namespace eShopLegacy.DAL
{
    public static class CatalogContextSeed
    {
        public static async Task SeedAsync(eShopContext context)
        {
            // Only seed if the tables are empty
            if (await context.CatalogBrands.AnyAsync()) return;

            // ── Brands ────────────────────────────────────────────────────────
            var brands = new List<CatalogBrand>
            {
                new() { Brand = ".NET"        },
                new() { Brand = "Azure"       },
                new() { Brand = "Visual Studio"},
                new() { Brand = "SQL Server"  },
                new() { Brand = "Other"       }
            };
            context.CatalogBrands.AddRange(brands);

            // ── Types ─────────────────────────────────────────────────────────
            var types = new List<CatalogType>
            {
                new() { Type = "Mug"       },
                new() { Type = "T-Shirt"   },
                new() { Type = "Sweatshirt"},
                new() { Type = "Sticker"   },
                new() { Type = "Pin Badge" }
            };
            context.CatalogTypes.AddRange(types);

            await context.SaveChangesAsync();

            // Reload to get DB-generated IDs
            var brandMap = await context.CatalogBrands.ToDictionaryAsync(b => b.Brand, b => b.Id);
            var typeMap  = await context.CatalogTypes.ToDictionaryAsync(t => t.Type,  t => t.Id);

            // ── Products ──────────────────────────────────────────────────────
            var items = new List<CatalogItem>
            {
                new()
                {
                    Name           = ".NET Black & White Mug",
                    Description    = "Stylish black and white mug with the .NET logo.",
                    Price          = 8.50m,
                    PictureFileName= "1.png",
                    PictureUri     = "https://raw.githubusercontent.com/dotnet-architecture/eShopOnContainers/dev/src/Services/Catalog/Catalog.API/Pics/1.png",
                    CatalogBrandId = brandMap[".NET"],
                    CatalogTypeId  = typeMap["Mug"],
                    AvailableStock = 100
                },
                new()
                {
                    Name           = ".NET Foundation Mug",
                    Description    = "Celebrate the .NET Foundation with this exclusive mug.",
                    Price          = 9.00m,
                    PictureFileName= "2.png",
                    PictureUri     = "https://raw.githubusercontent.com/dotnet-architecture/eShopOnContainers/dev/src/Services/Catalog/Catalog.API/Pics/2.png",
                    CatalogBrandId = brandMap[".NET"],
                    CatalogTypeId  = typeMap["Mug"],
                    AvailableStock = 80
                },
                new()
                {
                    Name           = ".NET Bot Black Sweatshirt",
                    Description    = "Comfortable black sweatshirt featuring the iconic .NET Bot.",
                    Price          = 19.50m,
                    PictureFileName= "3.png",
                    PictureUri     = "https://raw.githubusercontent.com/dotnet-architecture/eShopOnContainers/dev/src/Services/Catalog/Catalog.API/Pics/3.png",
                    CatalogBrandId = brandMap[".NET"],
                    CatalogTypeId  = typeMap["Sweatshirt"],
                    AvailableStock = 50
                },
                new()
                {
                    Name           = ".NET Bot White Sweatshirt",
                    Description    = "Classic white sweatshirt with .NET Bot graphic.",
                    Price          = 19.50m,
                    PictureFileName= "4.png",
                    PictureUri     = "https://raw.githubusercontent.com/dotnet-architecture/eShopOnContainers/dev/src/Services/Catalog/Catalog.API/Pics/4.png",
                    CatalogBrandId = brandMap[".NET"],
                    CatalogTypeId  = typeMap["Sweatshirt"],
                    AvailableStock = 45
                },
                new()
                {
                    Name           = "Azure Logo Mug",
                    Description    = "Show your cloud passion with this Azure-branded mug.",
                    Price          = 9.50m,
                    PictureFileName= "5.png",
                    PictureUri     = "https://raw.githubusercontent.com/dotnet-architecture/eShopOnContainers/dev/src/Services/Catalog/Catalog.API/Pics/5.png",
                    CatalogBrandId = brandMap["Azure"],
                    CatalogTypeId  = typeMap["Mug"],
                    AvailableStock = 120
                },
                new()
                {
                    Name           = "Azure Logo T-Shirt",
                    Description    = "Lightweight Azure-branded T-Shirt for developers.",
                    Price          = 12.00m,
                    PictureFileName= "6.png",
                    PictureUri     = "https://raw.githubusercontent.com/dotnet-architecture/eShopOnContainers/dev/src/Services/Catalog/Catalog.API/Pics/6.png",
                    CatalogBrandId = brandMap["Azure"],
                    CatalogTypeId  = typeMap["T-Shirt"],
                    AvailableStock = 70
                },
                new()
                {
                    Name           = "Visual Studio Pro T-Shirt",
                    Description    = "Soft cotton T-Shirt for Visual Studio power users.",
                    Price          = 14.00m,
                    PictureFileName= "7.png",
                    PictureUri     = "https://raw.githubusercontent.com/dotnet-architecture/eShopOnContainers/dev/src/Services/Catalog/Catalog.API/Pics/7.png",
                    CatalogBrandId = brandMap["Visual Studio"],
                    CatalogTypeId  = typeMap["T-Shirt"],
                    AvailableStock = 60
                },
                new()
                {
                    Name           = "SQL Server Sticker Pack",
                    Description    = "A pack of high-quality SQL Server logo stickers.",
                    Price          = 3.50m,
                    PictureFileName= "8.png",
                    PictureUri     = "https://raw.githubusercontent.com/dotnet-architecture/eShopOnContainers/dev/src/Services/Catalog/Catalog.API/Pics/8.png",
                    CatalogBrandId = brandMap["SQL Server"],
                    CatalogTypeId  = typeMap["Sticker"],
                    AvailableStock = 200
                },
                new()
                {
                    Name           = ".NET Sticker Set",
                    Description    = "Collection of .NET logo stickers in various sizes.",
                    Price          = 3.00m,
                    PictureFileName= "9.png",
                    PictureUri     = "https://raw.githubusercontent.com/dotnet-architecture/eShopOnContainers/dev/src/Services/Catalog/Catalog.API/Pics/9.png",
                    CatalogBrandId = brandMap[".NET"],
                    CatalogTypeId  = typeMap["Sticker"],
                    AvailableStock = 250
                },
                new()
                {
                    Name           = "Azure Pin Badge",
                    Description    = "Enamel pin badge with the Azure logo — perfect for lanyards.",
                    Price          = 5.00m,
                    PictureFileName= "10.png",
                    PictureUri     = "https://raw.githubusercontent.com/dotnet-architecture/eShopOnContainers/dev/src/Services/Catalog/Catalog.API/Pics/10.png",
                    CatalogBrandId = brandMap["Azure"],
                    CatalogTypeId  = typeMap["Pin Badge"],
                    AvailableStock = 150
                },
                new()
                {
                    Name           = "Visual Studio Mug",
                    Description    = "Start your morning right with this Visual Studio mug.",
                    Price          = 8.50m,
                    PictureFileName= "11.png",
                    PictureUri     = "https://raw.githubusercontent.com/dotnet-architecture/eShopOnContainers/dev/src/Services/Catalog/Catalog.API/Pics/11.png",
                    CatalogBrandId = brandMap["Visual Studio"],
                    CatalogTypeId  = typeMap["Mug"],
                    AvailableStock = 90
                },
                new()
                {
                    Name           = ".NET Foundation Pin Badge",
                    Description    = "Show your .NET Foundation support with this enamel badge.",
                    Price          = 5.50m,
                    PictureFileName= "12.png",
                    PictureUri     = "https://raw.githubusercontent.com/dotnet-architecture/eShopOnContainers/dev/src/Services/Catalog/Catalog.API/Pics/12.png",
                    CatalogBrandId = brandMap[".NET"],
                    CatalogTypeId  = typeMap["Pin Badge"],
                    AvailableStock = 110
                }
            };

            context.CatalogItems.AddRange(items);
            await context.SaveChangesAsync();
        }
    }
}
