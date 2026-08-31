using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using eShop.Web.Services;
using eShopLegacy.DAL;
using eShopLegacy.Models;

namespace eShop.Web.Pages.Catalog
{
    public class CatalogModel : PageModel
    {
        private const int PageSize = 8;
        private readonly CatalogService _catalog;
        private readonly BasketService _basket;
        private readonly IBuyerIdAccessor _buyer;

        public CatalogModel(CatalogService catalog, BasketService basket, IBuyerIdAccessor buyer)
        {
            _catalog = catalog;
            _basket = basket;
            _buyer = buyer;
        }

        public List<CatalogItem> Items { get; private set; }
        public List<SelectListItem> BrandOptions { get; private set; }
        public List<SelectListItem> TypeOptions { get; private set; }
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }
        public int Brand { get; private set; }
        public int Type { get; private set; }
        public string Search { get; private set; }

        public void OnGet(int page = 0, int brand = 0, int type = 0, string q = "")
        {
            PageIndex = Math.Max(0, page);
            Brand = brand;
            Type = type;
            Search = (q ?? "").Trim();
            Items = _catalog.GetCatalogItems(PageIndex, PageSize, brand == 0 ? null : brand, type == 0 ? null : type, Search, out var total);
            TotalPages = Math.Max(1, (int)Math.Ceiling((double)total / PageSize));
            BrandOptions = Options("All Brands", brand, _catalog.GetCatalogBrands().ConvertAll(x => (x.Id, x.Brand)));
            TypeOptions = Options("All Types", type, _catalog.GetCatalogTypes().ConvertAll(x => (x.Id, x.Type)));
        }

        public IActionResult OnPostAdd(int itemId, int page = 0, int brand = 0, int type = 0, string q = "")
        {
            var item = _catalog.GetCatalogItem(itemId);
            if (item != null) _basket.AddItemToBasket(_buyer.GetBuyerId(), item.Id, item.Price);
            return Redirect(PageUrl(page) + (PageUrl(page).Contains("?") ? "&" : "?") + "added=1");
        }

        public string PageUrl(int page)
        {
            var values = new List<string>();
            if (page > 0) values.Add("page=" + page);
            if (Brand > 0) values.Add("brand=" + Brand);
            if (Type > 0) values.Add("type=" + Type);
            if (!string.IsNullOrEmpty(Search)) values.Add("q=" + Uri.EscapeDataString(Search));
            return "/Catalog/Default.aspx" + (values.Count == 0 ? "" : "?" + string.Join("&", values));
        }

        private static List<SelectListItem> Options(string allText, int selected, List<(int Id, string Text)> values)
        {
            var result = new List<SelectListItem> { new SelectListItem(allText, "0", selected == 0) };
            foreach (var value in values) result.Add(new SelectListItem(value.Text, value.Id.ToString(), value.Id == selected));
            return result;
        }
    }
}