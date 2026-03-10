using System;
using System.Web;
using System.Web.Mvc;
using eShopLegacy.DAL;

namespace eShopLegacy.Controllers
{
    public abstract class BaseController : Controller
    {
        protected string GetBuyerId()
        {
            if (User.Identity.IsAuthenticated)
                return User.Identity.Name;

            if (Session["AnonymousBuyerId"] == null)
                Session["AnonymousBuyerId"] = Guid.NewGuid().ToString();

            return Session["AnonymousBuyerId"].ToString();
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            // Populate cart badge count for every request
            string buyerId = null;
            if (User.Identity.IsAuthenticated)
                buyerId = User.Identity.Name;
            else if (Session["AnonymousBuyerId"] != null)
                buyerId = Session["AnonymousBuyerId"].ToString();

            if (buyerId != null)
            {
                using (var ctx = new eShopContext())
                {
                    var count = new BasketService(ctx).GetBasketItemCount(buyerId);
                    ViewBag.CartItemCount = count > 0 ? (int?)count : null;
                }
            }
        }
    }
}
