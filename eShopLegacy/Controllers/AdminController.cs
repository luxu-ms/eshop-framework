using System.Web.Mvc;
using eShopLegacy.DAL;
using eShopLegacy.Models;
using eShopLegacy.Models.ViewModels;

namespace eShopLegacy.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : BaseController
    {
        public ActionResult Products(string success = null)
        {
            var vm = BuildViewModel();
            vm.SuccessMessage = success;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(ProductFormViewModel form)
        {
            if (!ModelState.IsValid)
            {
                var vm = BuildViewModel();
                vm.Form = form;
                return View("Products", vm);
            }

            using (var ctx = new eShopContext())
            {
                new CatalogService(ctx).AddCatalogItem(new CatalogItem
                {
                    Name           = form.Name.Trim(),
                    Description    = form.Description?.Trim(),
                    Price          = form.Price,
                    CatalogBrandId = form.CatalogBrandId,
                    CatalogTypeId  = form.CatalogTypeId,
                    AvailableStock = form.AvailableStock
                });
            }

            return RedirectToAction("Products", new { success = "Product added successfully." });
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            using (var ctx = new eShopContext())
            {
                var item = ctx.CatalogItems.Find(id);
                if (item == null) return HttpNotFound();

                var vm = BuildViewModel();
                vm.Form = new ProductFormViewModel
                {
                    Id             = item.Id,
                    Name           = item.Name,
                    Description    = item.Description,
                    Price          = item.Price,
                    CatalogBrandId = item.CatalogBrandId,
                    CatalogTypeId  = item.CatalogTypeId,
                    AvailableStock = item.AvailableStock
                };
                return View("Products", vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductFormViewModel form)
        {
            if (!ModelState.IsValid)
            {
                var vm = BuildViewModel();
                vm.Form = form;
                return View("Products", vm);
            }

            using (var ctx = new eShopContext())
            {
                var item = ctx.CatalogItems.Find(form.Id);
                if (item != null)
                {
                    item.Name           = form.Name.Trim();
                    item.Description    = form.Description?.Trim();
                    item.Price          = form.Price;
                    item.CatalogBrandId = form.CatalogBrandId;
                    item.CatalogTypeId  = form.CatalogTypeId;
                    item.AvailableStock = form.AvailableStock;
                    new CatalogService(ctx).UpdateCatalogItem(item);
                }
            }

            return RedirectToAction("Products", new { success = "Product updated successfully." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            using (var ctx = new eShopContext())
                new CatalogService(ctx).DeleteCatalogItem(id);
            return RedirectToAction("Products", new { success = "Product deleted." });
        }

        private AdminProductsViewModel BuildViewModel()
        {
            using (var ctx = new eShopContext())
            {
                var svc = new CatalogService(ctx);
                int total;
                return new AdminProductsViewModel
                {
                    Products = svc.GetCatalogItems(0, int.MaxValue, null, null, string.Empty, out total),
                    Brands   = svc.GetCatalogBrands(),
                    Types    = svc.GetCatalogTypes()
                };
            }
        }
    }
}
