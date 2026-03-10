using System;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using eShopLegacy.App_Start;
using eShopLegacy.DAL;
using eShopLegacy.Models;
using eShopLegacy.Models.ViewModels;

namespace eShopLegacy.Controllers
{
    public class AccountController : BaseController
    {
        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToLocal(returnUrl);

            var vm = new LoginViewModel();

            // Pre-fill email from remember-me cookie
            var rememberedEmail = Request.Cookies["remember_email"]?.Value;
            if (!string.IsNullOrEmpty(rememberedEmail))
            {
                vm.Email = HttpUtility.HtmlDecode(rememberedEmail);
                vm.RememberMe = true;
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            var manager = IdentityConfig.CreateUserManager();
            var user = manager.Find(model.Email.Trim(), model.Password);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            var authManager = HttpContext.GetOwinContext().Authentication;
            var identity = manager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
            authManager.SignIn(new Microsoft.Owin.Security.AuthenticationProperties
            {
                IsPersistent = model.RememberMe
            }, identity);

            // Save or clear the remember-me cookie
            if (model.RememberMe)
            {
                var cookie = new HttpCookie("remember_email", HttpUtility.HtmlEncode(model.Email.Trim()))
                {
                    Expires = DateTime.Now.AddDays(30),
                    HttpOnly = true
                };
                Response.Cookies.Set(cookie);
            }
            else
            {
                Response.Cookies.Set(new HttpCookie("remember_email") { Expires = DateTime.Now.AddDays(-1) });
            }

            TransferAnonymousBasket(user.UserName);

            return RedirectToLocal(returnUrl);
        }

        [HttpGet]
        public ActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Catalog");
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var manager = IdentityConfig.CreateUserManager();
            var user = new ApplicationUser
            {
                UserName = model.Email.Trim(),
                Email    = model.Email.Trim(),
                Name     = model.FirstName.Trim(),
                LastName = model.LastName.Trim()
            };

            var result = manager.Create(user, model.Password);
            if (result.Succeeded)
            {
                var authManager = HttpContext.GetOwinContext().Authentication;
                var identity = manager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
                authManager.SignIn(new Microsoft.Owin.Security.AuthenticationProperties { IsPersistent = false }, identity);
                return RedirectToAction("Index", "Catalog");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            Session.Clear();
            return RedirectToAction("Index", "Catalog");
        }

        private void TransferAnonymousBasket(string userId)
        {
            string anonId = Session["AnonymousBuyerId"]?.ToString();
            if (string.IsNullOrEmpty(anonId)) return;

            using (var ctx = new eShopContext())
            {
                var svc = new BasketService(ctx);
                var anonBasket = svc.GetBasket(anonId);
                if (anonBasket?.Items == null) return;

                foreach (var item in anonBasket.Items)
                    svc.AddItemToBasket(userId, item.CatalogItemId, item.UnitPrice, item.Quantity);

                svc.ClearBasket(anonId);
            }

            Session.Remove("AnonymousBuyerId");
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction("Index", "Catalog");
        }
    }
}
