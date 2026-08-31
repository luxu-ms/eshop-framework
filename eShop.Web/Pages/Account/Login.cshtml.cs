using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using eShopLegacy.DAL;
using eShopLegacy.Models;

namespace eShop.Web.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly CommerceContext _context;
        private readonly BasketService _basket;
        private readonly IPasswordHasher<UserRecord> _hasher;
        public LoginModel(CommerceContext context, BasketService basket, IPasswordHasher<UserRecord> hasher) { _context = context; _basket = basket; _hasher = hasher; }
        [BindProperty, Required, EmailAddress, Display(Name = "Email address")] public string Email { get; set; }
        [BindProperty, Required, DataType(DataType.Password)] public string Password { get; set; }
        [BindProperty, Display(Name = "Remember me")] public bool Remember { get; set; }
        [BindProperty(SupportsGet = true)] public string ReturnUrl { get; set; }

        public IActionResult OnGet()
        {
            if (User.Identity?.IsAuthenticated == true) return Redirect("/");
            if (Request.Cookies.TryGetValue("remember_email", out var email)) { Email = email; Remember = true; }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = _context.Users.FirstOrDefault(x => x.UserName == Email);
            if (!ModelState.IsValid || user == null || _hasher.VerifyHashedPassword(user, user.PasswordHash, Password) == PasswordVerificationResult.Failed)
            { ModelState.AddModelError(string.Empty, "Invalid email or password."); return Page(); }

            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, user.Id), new Claim(ClaimTypes.Name, user.UserName) }, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), new AuthenticationProperties { IsPersistent = Remember });
            if (Remember) Response.Cookies.Append("remember_email", Email, new Microsoft.AspNetCore.Http.CookieOptions { HttpOnly = true, MaxAge = System.TimeSpan.FromDays(30) }); else Response.Cookies.Delete("remember_email");
            TransferBasket(user.UserName);
            return Url.IsLocalUrl(ReturnUrl) ? LocalRedirect(ReturnUrl) : Redirect("/");
        }

        private void TransferBasket(string userName)
        {
            if (!Request.Cookies.TryGetValue("eshop-anon-id", out var anonymousId)) return;
            var anonymous = _basket.GetBasket(anonymousId);
            if (anonymous?.Items == null) return;
            foreach (var item in new List<BasketItem>(anonymous.Items)) _basket.AddItemToBasket(userName, item.CatalogItemId, item.UnitPrice, item.Quantity);
            _basket.ClearBasket(anonymousId);
            Response.Cookies.Delete("eshop-anon-id");
        }
    }
}