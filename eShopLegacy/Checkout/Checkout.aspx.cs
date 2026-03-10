using System;
using System.Linq;
using System.Web.UI;
using eShopLegacy.DAL;
using eShopLegacy.Models;

namespace eShopLegacy.Checkout
{
    public partial class CheckoutPage : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!User.Identity.IsAuthenticated)
                Response.Redirect("~/Account/Login.aspx?ReturnUrl=%2fCheckout%2fCheckout.aspx");

            if (!IsPostBack)
            {
                PreFillFromProfile();
                BindOrderSummary();
            }
        }

        private void PreFillFromProfile()
        {
            using (var ctx = new eShopContext())
            {
                var user = ctx.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
                if (user == null) return;

                txtStreet.Text  = user.Street  ?? "";
                txtCity.Text    = user.City    ?? "";
                txtState.Text   = user.State   ?? "";
                txtZip.Text     = user.ZipCode ?? "";
                txtCountry.Text = user.Country ?? "";
            }
        }

        private void BindOrderSummary()
        {
            using (var ctx = new eShopContext())
            {
                var basket = new BasketService(ctx).GetBasket(User.Identity.Name);
                if (basket == null || !basket.Items.Any())
                {
                    Response.Redirect("~/Cart/ShoppingCart.aspx");
                    return;
                }

                var items = basket.Items.ToList();
                rptSummary.DataSource = items;
                rptSummary.DataBind();
                lblTotal.Text = items.Sum(i => i.UnitPrice * i.Quantity).ToString("0.00");
            }
        }

        protected void btnPlaceOrder_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            try
            {
                // Parse expiry date
                DateTime cardExp = DateTime.UtcNow.AddYears(1);
                if (!string.IsNullOrEmpty(txtExpiry.Text))
                {
                    if (!DateTime.TryParseExact("01/" + txtExpiry.Text, "dd/MM/yy",
                        System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.None, out cardExp))
                    {
                        cardExp = DateTime.UtcNow.AddYears(1);
                    }
                }

                var address = new Address
                {
                    Street  = txtStreet.Text.Trim(),
                    City    = txtCity.Text.Trim(),
                    State   = txtState.Text.Trim(),
                    ZipCode = txtZip.Text.Trim(),
                    Country = txtCountry.Text.Trim()
                };

                using (var ctx = new eShopContext())
                {
                    var orderSvc = new OrderService(ctx);
                    var order = orderSvc.CreateOrderFromBasket(
                        buyerId:           User.Identity.Name,
                        buyerName:         User.Identity.Name,
                        shippingAddress:   address,
                        cardNumber:        txtCardNumber.Text.Trim(),
                        cardHolderName:    txtCardHolder.Text.Trim(),
                        cardExpiration:    cardExp,
                        cardSecurityNumber:txtCVV.Text.Trim(),
                        cardTypeId:        1);

                    Response.Redirect("~/Checkout/OrderComplete.aspx?orderId=" + order.Id);
                }
            }
            catch (Exception ex)
            {
                pnlError.Visible = true;
                litError.Text = Server.HtmlEncode("An error occurred placing your order: " + ex.Message);
            }
        }
    }
}
