using System.Collections.Generic;

namespace eShopLegacy.Models
{
    public class Basket
    {
        public int Id { get; set; }

        public string BuyerId { get; set; }

        public virtual ICollection<BasketItem> Items { get; set; } = new List<BasketItem>();
    }
}
