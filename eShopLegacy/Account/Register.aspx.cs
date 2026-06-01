using System;
using System.Web;
using System.Web.UI;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;

namespace eShopLegacy.Account
{
    // With Microsoft Entra ID, user registration is managed by Azure AD.
    // This page redirects unauthenticated users to the Entra ID login page,
    // and authenticated users to their account/profile page.
    public partial class RegisterPage : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Request.IsAuthenticated)
            {
                // Trigger Azure AD / Entra ID sign-up / sign-in flow
                HttpContext.Current.GetOwinContext().Authentication.Challenge(
                    new AuthenticationProperties { RedirectUri = "/" },
                    OpenIdConnectAuthenticationDefaults.AuthenticationType);
            }
            else
            {
                Response.Redirect("~/");
            }
        }
    }
}
