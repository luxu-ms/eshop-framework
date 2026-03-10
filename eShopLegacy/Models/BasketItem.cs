using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eShopLegacy.Models
{
    public class BasketItem
    {
        public int Id { get; set; }

        public int BasketId { get; set; }

        public int CatalogItemId { get; set; }

        [Required]
        public string ProductName { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal OldUnitPrice { get; set; }

        public int Quantity { get; set; }

        public string PictureUrl { get; set; }

        public virtual Basket Basket { get; set; }

        [ForeignKey("CatalogItemId")]
        public virtual CatalogItem CatalogItem { get; set; }
    }
}
