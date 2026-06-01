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
            if (!Request.IsAuthenticated)
            {
                // Trigger Azure AD / Microsoft Entra ID login
                HttpContext.Current.GetOwinContext().Authentication.Challenge(
                    new AuthenticationProperties { RedirectUri = "/" },
                    OpenIdConnectAuthenticationDefaults.AuthenticationType);
            }
            else
            {
                // Already authenticated — redirect home or to ReturnUrl
                string returnUrl = Request.QueryString["ReturnUrl"];
                if (!string.IsNullOrEmpty(returnUrl))
                    Response.Redirect(returnUrl);
                else
                    Response.Redirect("~/");
            }
        }
    }
}
