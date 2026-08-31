using System;
using Microsoft.AspNetCore.Http;

namespace eShop.Web.Services
{
    public interface IBuyerIdAccessor
    {
        string GetBuyerId();
    }

    public class BuyerIdAccessor : IBuyerIdAccessor
    {
        private const string CookieName = "eshop-anon-id";
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BuyerIdAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetBuyerId()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context.User.Identity?.IsAuthenticated == true)
                return context.User.Identity.Name;

            if (context.Request.Cookies.TryGetValue(CookieName, out var buyerId) && !string.IsNullOrEmpty(buyerId))
                return buyerId;

            buyerId = Guid.NewGuid().ToString();
            context.Response.Cookies.Append(CookieName, buyerId, new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Lax,
                IsEssential = true
            });
            return buyerId;
        }
    }
}