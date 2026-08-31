using System;
using System.Web;
using System.Web.UI;

namespace eShopLegacy.App_Start
{
    public static class BuyerIdAccessor
    {
        private const string CookieName = "eshop-anon-id";
        private const string SessionKey = "AnonymousBuyerId";

        public static string Get(Page page)
        {
            if (page.User.Identity.IsAuthenticated)
                return page.User.Identity.Name;

            var buyerId = GetAnonymous(page);
            if (string.IsNullOrEmpty(buyerId))
            {
                buyerId = Guid.NewGuid().ToString();
                page.Session[SessionKey] = buyerId;
                SetCookie(page, buyerId);
            }

            return buyerId;
        }

        public static string GetAnonymous(Page page)
        {
            var sessionBuyerId = page.Session[SessionKey]?.ToString();
            var cookieBuyerId = page.Request.Cookies[CookieName]?.Value;
            var buyerId = sessionBuyerId ?? cookieBuyerId;

            if (!string.IsNullOrEmpty(buyerId))
            {
                page.Session[SessionKey] = buyerId;
                if (cookieBuyerId != buyerId)
                    SetCookie(page, buyerId);
            }

            return buyerId;
        }

        private static void SetCookie(Page page, string buyerId)
        {
            page.Response.Cookies.Set(new HttpCookie(CookieName, buyerId)
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Lax
            });
        }
    }
}