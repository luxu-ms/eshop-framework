using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using eShopLegacy.Models;

namespace eShopLegacy.Models.ViewModels
{
    public class AdminProductsViewModel
    {
        public List<CatalogItem> Products { get; set; } = new List<CatalogItem>();
        public List<CatalogBrand> Brands { get; set; } = new List<CatalogBrand>();
        public List<CatalogType> Types { get; set; } = new List<CatalogType>();

        public ProductFormViewModel Form { get; set; } = new ProductFormViewModel();
        public string SuccessMessage { get; set; }
    }

    public class ProductFormViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [Range(0.01, 10000)]
        public decimal Price { get; set; }

        [Required]
        [Display(Name = "Brand")]
        public int CatalogBrandId { get; set; }

        [Required]
        [Display(Name = "Type")]
        public int CatalogTypeId { get; set; }

        [Display(Name = "Stock")]
        public int AvailableStock { get; set; }
    }
}
