using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eShopLegacy.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }

        [Required]
        [StringLength(180)]
        public string ProductName { get; set; }

        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public int Units { get; set; }
        public string PictureUrl { get; set; }
        public virtual Order Order { get; set; }

        [ForeignKey("ProductId")]
        public virtual CatalogItem CatalogItem { get; set; }
    }
}