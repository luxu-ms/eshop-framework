using System;
using System.Linq;
using System.Web.Mvc;
using eShopLegacy.DAL;
using eShopLegacy.Models;
using eShopLegacy.Models.ViewModels;

namespace eShopLegacy.Controllers
{
    [Authorize]
    public class CheckoutController : BaseController
    {
        [HttpGet]
        public ActionResult Index()
        {
            using (var ctx = new eShopContext())
            {
                var basket = new BasketService(ctx).GetBasket(User.Identity.Name);
                if (basket == null || !basket.Items.Any())
                    return RedirectToAction("Index", "Cart");

                var vm = new CheckoutViewModel
                {
                    OrderItems = basket.Items.ToList()
                };

                // Pre-fill from user profile
                var user = ctx.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
                if (user != null)
                {
                    vm.Street  = user.Street  ?? "";
                    vm.City    = user.City    ?? "";
                    vm.State   = user.State   ?? "";
                    vm.ZipCode = user.ZipCode ?? "";
                    vm.Country = user.Country ?? "";
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(CheckoutViewModel model)
        {
            // Re-populate order items for display on validation failure
            using (var ctx = new eShopContext())
            {
                var basket = new BasketService(ctx).GetBasket(User.Identity.Name);
                if (basket != null)
                    model.OrderItems = basket.Items.ToList();
            }

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                DateTime cardExp = DateTime.UtcNow.AddYears(1);
                if (!string.IsNullOrEmpty(model.CardExpiry))
                {
                    DateTime.TryParseExact("01/" + model.CardExpiry, "dd/MM/yy",
                        System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.None, out cardExp);
                }

                var address = new Address
                {
                    Street  = model.Street,
                    City    = model.City,
                    State   = model.State,
                    ZipCode = model.ZipCode,
                    Country = model.Country
                };

                using (var ctx = new eShopContext())
                {
                    var order = new OrderService(ctx).CreateOrderFromBasket(
                        buyerId:            User.Identity.Name,
                        buyerName:          User.Identity.Name,
                        shippingAddress:    address,
                        cardNumber:         model.CardNumber,
                        cardHolderName:     model.CardHolderName,
                        cardExpiration:     cardExp,
                        cardSecurityNumber: model.CardSecurityNumber,
                        cardTypeId:         1);

                    return RedirectToAction("Complete", new { orderId = order.Id });
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred placing your order: " + ex.Message);
                return View(model);
            }
        }

        public ActionResult Complete(int orderId)
        {
            using (var ctx = new eShopContext())
            {
                var order = new OrderService(ctx).GetOrder(orderId, User.Identity.Name);
                if (order == null) return RedirectToAction("Index", "Catalog");

                var vm = new OrderCompleteViewModel
                {
                    OrderId   = order.Id,
                    OrderDate = order.OrderDate.ToLocalTime().ToString("f"),
                    Total     = order.Total,
                    Items     = order.OrderItems
                };
                return View(vm);
            }
        }

        public ActionResult History()
        {
            using (var ctx = new eShopContext())
            {
                var orders = new OrderService(ctx).GetOrdersForBuyer(User.Identity.Name);
                return View(new OrderHistoryViewModel { Orders = orders });
            }
        }
    }
}
