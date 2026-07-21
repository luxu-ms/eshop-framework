using System;
using System.Web;
using System.Web.UI;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using eShopLegacy.DAL;
using eShopLegacy.Models;

namespace eShopLegacy.Account
{
    public partial class LoginPage : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.IsAuthenticated)
            {
                string returnUrl = Request.QueryString["ReturnUrl"];
                string redirectTo = !string.IsNullOrEmpty(returnUrl) ? returnUrl : ResolveUrl("~/");
                Response.Redirect(redirectTo, false);
                Context.ApplicationInstance.CompleteRequest();
            }
        }

        protected void LoginButton_Click(object sender, EventArgs e)
        {
            string email = Email.Text.Trim();
            string password = Password.Text;

            using (var context = new eShopContext())
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);

                var user = userManager.FindByEmail(email);
                if (user != null && userManager.CheckPassword(user, password))
                {
                    var identity = userManager.CreateIdentity(user, CookieAuthenticationDefaults.AuthenticationType);
                    var authManager = HttpContext.Current.GetOwinContext().Authentication;
                    authManager.SignIn(new AuthenticationProperties { IsPersistent = true }, identity);

                    string returnUrl = Request.QueryString["ReturnUrl"];
                    string redirectTo = !string.IsNullOrEmpty(returnUrl) ? returnUrl : ResolveUrl("~/");
                    // Use endResponse=false so OWIN can write the Set-Cookie header
                    Response.Redirect(redirectTo, false);
                    Context.ApplicationInstance.CompleteRequest();
                }
                else
                {
                    ErrorPanel.Visible = true;
                    ErrorMessage.Text = "Invalid email or password.";
                }
            }
        }
    }
}
