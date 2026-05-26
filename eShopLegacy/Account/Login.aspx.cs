using System;
using System.Web;
using System.Web.UI;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using eShopLegacy.DAL;

namespace eShopLegacy.Account
{
    public partial class LoginPage : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (User.Identity.IsAuthenticated)
            {
                // User already signed in via Entra ID – transfer basket then redirect
                TransferAnonymousBasket(User.Identity.Name);
                Response.Redirect("~/");
                return;
            }

            // Trigger the Microsoft Entra ID OIDC sign-in challenge.
            // The OWIN middleware will redirect the user to Azure AD login.
            string returnUrl = Request.QueryString["ReturnUrl"] ?? "/";
            HttpContext.Current.GetOwinContext().Authentication.Challenge(
                new AuthenticationProperties { RedirectUri = returnUrl },
                OpenIdConnectAuthenticationDefaults.AuthenticationType);
        }

        // btnLogin_Click is no longer used – authentication is delegated to Entra ID.
        // Retained as empty handler to avoid breaking the ASPX code-behind binding.
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            // No-op: login is initiated by Page_Load via OIDC challenge.
        }

        private void TransferAnonymousBasket(string userId)
        {
            string anonId = Session["AnonymousBuyerId"]?.ToString();
            if (string.IsNullOrEmpty(anonId)) return;

            using (var ctx = new eShopContext())
            {
                var svc = new BasketService(ctx);
                var anonBasket = svc.GetBasket(anonId);
                if (anonBasket == null || anonBasket.Items == null) return;

                foreach (var item in anonBasket.Items)
                    svc.AddItemToBasket(userId, item.CatalogItemId, item.UnitPrice, item.Quantity);

                svc.ClearBasket(anonId);
            }

            Session.Remove("AnonymousBuyerId");
        }
    }
}
