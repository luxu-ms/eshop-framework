using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eShopLegacy.Models
{
    public class CatalogItem
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public decimal Price { get; set; }
        public string PictureFileName { get; set; }
        public string PictureUri { get; set; }
        public int CatalogTypeId { get; set; }

        [ForeignKey("CatalogTypeId")]
        public virtual CatalogType CatalogType { get; set; }

        public int CatalogBrandId { get; set; }

        [ForeignKey("CatalogBrandId")]
        public virtual CatalogBrand CatalogBrand { get; set; }

        public int AvailableStock { get; set; }
        public int RestockThreshold { get; set; }
        public int MaxStockThreshold { get; set; }
        public bool OnReorder { get; set; }
    }
}