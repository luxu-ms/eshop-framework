using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using eShop.Web.Services;
using eShopLegacy.DAL;
using eShopLegacy.Models;

namespace eShop.Web.Pages.Cart
{
    public class ShoppingCartModel : PageModel
    {
        private readonly BasketService _basket;
        private readonly CommerceContext _context;
        private readonly IBuyerIdAccessor _buyer;
        public ShoppingCartModel(BasketService basket, CommerceContext context, IBuyerIdAccessor buyer) { _basket = basket; _context = context; _buyer = buyer; }
        public List<BasketItem> Items { get; private set; } = new List<BasketItem>();
        public decimal Total { get; private set; }
        public void OnGet() { Load(); }
        public IActionResult OnPostChange(int itemId, int delta) { var item = _context.BasketItems.Find(itemId); if (item != null) _basket.UpdateBasketItem(itemId, item.Quantity + delta); return RedirectToPage(); }
        public IActionResult OnPostRemove(int itemId) { _basket.RemoveItemFromBasket(itemId); return RedirectToPage(); }
        private void Load() { Items = _basket.GetBasket(_buyer.GetBuyerId())?.Items.ToList() ?? new List<BasketItem>(); Total = Items.Sum(x => x.UnitPrice * x.Quantity); }
    }
}