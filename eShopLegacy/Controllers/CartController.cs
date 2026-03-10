using System.Linq;
using System.Web.Mvc;
using eShopLegacy.DAL;
using eShopLegacy.Models.ViewModels;

namespace eShopLegacy.Controllers
{
    public class CartController : BaseController
    {
        public ActionResult Index()
        {
            string buyerId = GetBuyerId();
            using (var ctx = new eShopContext())
            {
                var basket = new BasketService(ctx).GetBasket(buyerId);
                var vm = new ShoppingCartViewModel
                {
                    Items = basket?.Items?.ToList() ?? new System.Collections.Generic.List<eShopLegacy.Models.BasketItem>()
                };
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Increment(int basketItemId)
        {
            using (var ctx = new eShopContext())
            {
                var item = ctx.BasketItems.Find(basketItemId);
                if (item != null)
                    new BasketService(ctx).UpdateBasketItem(basketItemId, item.Quantity + 1);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Decrement(int basketItemId)
        {
            using (var ctx = new eShopContext())
            {
                var item = ctx.BasketItems.Find(basketItemId);
                if (item != null)
                    new BasketService(ctx).UpdateBasketItem(basketItemId, item.Quantity - 1);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Remove(int basketItemId)
        {
            using (var ctx = new eShopContext())
                new BasketService(ctx).RemoveItemFromBasket(basketItemId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Checkout()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Index", "Checkout") });
            return RedirectToAction("Index", "Checkout");
        }
    }
}
