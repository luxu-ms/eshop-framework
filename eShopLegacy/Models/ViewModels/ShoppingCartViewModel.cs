using System.Collections.Generic;
using System.Linq;
using eShopLegacy.Models;

namespace eShopLegacy.Models.ViewModels
{
    public class ShoppingCartViewModel
    {
        public List<BasketItem> Items { get; set; } = new List<BasketItem>();

        public int TotalQuantity => Items?.Sum(i => i.Quantity) ?? 0;
        public decimal Subtotal => Items?.Sum(i => i.UnitPrice * i.Quantity) ?? 0;
        public decimal Total => Subtotal;
    }
}
