using System;
using System.Web;
using System.Web.UI;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;

namespace eShopLegacy.Account
{
    /// <summary>
    /// Registration is now managed through Microsoft Entra ID.
    /// New users must be provisioned in the Azure Active Directory tenant by an administrator.
    /// This page redirects authenticated users to the home page and prompts unauthenticated
    /// users to sign in via Entra ID.
    /// </summary>
    public partial class RegisterPage : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (User.Identity.IsAuthenticated)
            {
                Response.Redirect("~/");
                return;
            }

            // Redirect to Entra ID sign-in for account access.
            // User provisioning is managed in the Azure Active Directory tenant.
            HttpContext.Current.GetOwinContext().Authentication.Challenge(
                new AuthenticationProperties { RedirectUri = "/" },
                OpenIdConnectAuthenticationDefaults.AuthenticationType);
        }

        // btnRegister_Click is no longer used – registration is managed in Entra ID.
        protected void btnRegister_Click(object sender, EventArgs e)
        {
            // No-op: user provisioning is handled in Microsoft Entra ID.
        }
    }
}
