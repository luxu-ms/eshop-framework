using System;
using System.Web;
using System.Web.UI;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using eShopLegacy.DAL;

namespace eShopLegacy
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                UpdateCartCount();
            }
        }

        private void UpdateCartCount()
        {
            string buyerId = GetBuyerId();
            if (string.IsNullOrEmpty(buyerId)) return;

            using (var ctx = new eShopContext())
            {
                var svc = new BasketService(ctx);
                int count = svc.GetBasketItemCount(buyerId);
                if (count > 0)
                {
                    lblCartCount.Text = count.ToString();
                    lblCartCount.Visible = true;
                }
            }
        }

        protected void btnSignOut_Click(object sender, EventArgs e)
        {
            Session.Clear();
            HttpContext.Current.GetOwinContext().Authentication.SignOut(
                CookieAuthenticationDefaults.AuthenticationType);
            Response.Redirect("~/");
        }

        private string GetBuyerId()
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
                return HttpContext.Current.User.Identity.Name;

            // Anonymous basket uses session
            if (Session["AnonymousBuyerId"] != null)
                return Session["AnonymousBuyerId"].ToString();

            var id = Guid.NewGuid().ToString();
            Session["AnonymousBuyerId"] = id;
            return id;
        }
    }
}
