using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using eShop.Web.Services;
using eShopLegacy.DAL;
using eShopLegacy.Models;

namespace eShop.Web.Pages.Catalog
{
    public class ProductDetailModel : PageModel
    {
        private readonly CatalogService _catalog;
        private readonly BasketService _basket;
        private readonly IBuyerIdAccessor _buyer;
        public ProductDetailModel(CatalogService catalog, BasketService basket, IBuyerIdAccessor buyer) { _catalog = catalog; _basket = basket; _buyer = buyer; }
        public CatalogItem Item { get; private set; }
        public bool Added { get; private set; }
        public void OnGet(int id) { Item = _catalog.GetCatalogItem(id); }
        public void OnPost(int id, int quantity = 1)
        {
            Item = _catalog.GetCatalogItem(id);
            if (Item == null) return;
            quantity = quantity < 1 ? 1 : quantity;
            _basket.AddItemToBasket(_buyer.GetBuyerId(), id, Item.Price, quantity);
            Added = true;
        }
    }
}