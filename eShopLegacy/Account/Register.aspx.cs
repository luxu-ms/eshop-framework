using System;
using System.Web;
using System.Web.UI;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using eShopLegacy.App_Start;
using eShopLegacy.Models;

namespace eShopLegacy.Account
{
    public partial class RegisterPage : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (User.Identity.IsAuthenticated)
                Response.Redirect("~/");
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            var manager = IdentityConfig.CreateUserManager();
            var user = new ApplicationUser
            {
                UserName = txtEmail.Text.Trim(),
                Email    = txtEmail.Text.Trim(),
                Name     = txtFirstName.Text.Trim(),
                LastName = txtLastName.Text.Trim()
            };

            var result = manager.Create(user, txtPassword.Text);
            if (result.Succeeded)
            {
                var authManager = Context.GetOwinContext().Authentication;
                var identity    = manager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
                authManager.SignIn(new Microsoft.Owin.Security.AuthenticationProperties
                {
                    IsPersistent = false
                }, identity);
                Response.Redirect("~/");
            }
            else
            {
                pnlError.Visible = true;
                litError.Text    = string.Join("<br/>", result.Errors);
            }
        }
    }
}
