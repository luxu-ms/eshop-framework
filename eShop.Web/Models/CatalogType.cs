using System.ComponentModel.DataAnnotations;

namespace eShopLegacy.Models
{
    public class CatalogType
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Type { get; set; }
    }
}