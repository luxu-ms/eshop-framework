using System;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using eShopLegacy.App_Start;
using eShopLegacy.DAL;

namespace eShopLegacy.Account
{
    public partial class LoginPage : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (User.Identity.IsAuthenticated)
                Response.Redirect("~/");

            if (!IsPostBack)
            {
                // Pre-fill email if the remember-me cookie exists
                var rememberedEmail = Request.Cookies["remember_email"]?.Value;
                if (!string.IsNullOrEmpty(rememberedEmail))
                {
                    txtEmail.Text = Server.HtmlDecode(rememberedEmail);
                    chkRemember.Checked = true;
                }
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            var manager = IdentityConfig.CreateUserManager();
            var user    = manager.Find(txtEmail.Text.Trim(), txtPassword.Text);

            if (user == null)
            {
                pnlError.Visible = true;
                litError.Text    = "Invalid email or password.";
                return;
            }

            var authManager = Context.GetOwinContext().Authentication;
            var identity    = manager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
            authManager.SignIn(new Microsoft.Owin.Security.AuthenticationProperties
            {
                IsPersistent = chkRemember.Checked
            }, identity);

            // Save or clear the remember-me email cookie
            if (chkRemember.Checked)
            {
                var cookie = new HttpCookie("remember_email", Server.HtmlEncode(txtEmail.Text.Trim()))
                {
                    Expires  = DateTime.Now.AddDays(30),
                    HttpOnly = true
                };
                Response.Cookies.Set(cookie);
            }
            else
            {
                // Expire any existing cookie
                var cookie = new HttpCookie("remember_email") { Expires = DateTime.Now.AddDays(-1) };
                Response.Cookies.Set(cookie);
            }

            // Transfer anonymous basket to user basket
            TransferAnonymousBasket(user.UserName);

            string returnUrl = Request.QueryString["ReturnUrl"];
            if (!string.IsNullOrEmpty(returnUrl))
                Response.Redirect(returnUrl);
            else
                Response.Redirect("~/");
        }

        private void TransferAnonymousBasket(string userId)
        {
            string anonId = Session["AnonymousBuyerId"]?.ToString();
            if (string.IsNullOrEmpty(anonId)) return;

            using (var ctx = new eShopContext())
            {
                var svc    = new BasketService(ctx);
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
