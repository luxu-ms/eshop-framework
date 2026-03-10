using System;
using System.Web.Mvc;
using eShopLegacy.DAL;
using eShopLegacy.Models.ViewModels;

namespace eShopLegacy.Controllers
{
    public class CatalogController : BaseController
    {
        private const int DefaultPageSize = 8;

        public ActionResult Index(int? page, int? brand, int? type, string q)
        {
            int pageIndex = Math.Max(0, page ?? 0);
            int? brandId  = brand == 0 ? null : brand;
            int? typeId   = type  == 0 ? null : type;
            string search  = (q ?? "").Trim();

            using (var ctx = new eShopContext())
            {
                var catalogSvc = new CatalogService(ctx);
                int total;
                var items  = catalogSvc.GetCatalogItems(pageIndex, DefaultPageSize, brandId, typeId, search, out total);
                var brands = catalogSvc.GetCatalogBrands();
                var types  = catalogSvc.GetCatalogTypes();

                var vm = new CatalogIndexViewModel
                {
                    Items          = items,
                    Brands         = brands,
                    Types          = types,
                    CurrentPage    = pageIndex,
                    TotalItems     = total,
                    TotalPages     = Math.Max(1, (int)Math.Ceiling((double)total / DefaultPageSize)),
                    PageSize       = DefaultPageSize,
                    SelectedBrandId = brand,
                    SelectedTypeId  = type,
                    SearchText     = search
                };

                return View(vm);
            }
        }

        public ActionResult Details(int id)
        {
            using (var ctx = new eShopContext())
            {
                var item = new CatalogService(ctx).GetCatalogItem(id);
                if (item == null) return HttpNotFound("Product not found.");
                return View(item);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddToCart(int id, int quantity = 1)
        {
            if (quantity < 1) quantity = 1;

            using (var ctx = new eShopContext())
            {
                var item = new CatalogService(ctx).GetCatalogItem(id);
                if (item == null) return HttpNotFound();

                string buyerId = GetBuyerId();
                new BasketService(ctx).AddItemToBasket(buyerId, id, item.Price, quantity);
            }

            return RedirectToAction("Index", "Cart");
        }
    }
}
