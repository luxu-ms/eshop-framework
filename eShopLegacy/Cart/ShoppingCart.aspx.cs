using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using eShopLegacy.App_Start;
using eShopLegacy.DAL;

namespace eShopLegacy.Cart
{
    public partial class ShoppingCartPage : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) BindCart();
        }

        private void BindCart()
        {
            string buyerId = GetBuyerId();
            using (var ctx = new eShopContext())
            {
                var basket = new BasketService(ctx).GetBasket(buyerId);
                var items  = basket?.Items.ToList();

                bool isEmpty = items == null || items.Count == 0;
                pnlEmpty.Visible = isEmpty;
                pnlCart.Visible  = !isEmpty;

                if (!isEmpty)
                {
                    rptCart.DataSource = items;
                    rptCart.DataBind();

                    int totalQty     = items.Sum(i => i.Quantity);
                    decimal subtotal = items.Sum(i => i.UnitPrice * i.Quantity);

                    lblItemCount.Text = totalQty.ToString();
                    lblSubtotal.Text  = subtotal.ToString("0.00");
                    lblTotal.Text     = subtotal.ToString("0.00");
                }
            }
        }

        protected void rptCart_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int itemId = int.Parse(e.CommandArgument.ToString());
            string buyerId = GetBuyerId();

            using (var ctx = new eShopContext())
            {
                var svc = new BasketService(ctx);
                if (e.CommandName == "Remove")
                    svc.RemoveItemFromBasket(itemId);
                else if (e.CommandName == "Increment")
                {
                    var item = ctx.BasketItems.Find(itemId);
                    if (item != null) svc.UpdateBasketItem(itemId, item.Quantity + 1);
                }
                else if (e.CommandName == "Decrement")
                {
                    var item = ctx.BasketItems.Find(itemId);
                    if (item != null) svc.UpdateBasketItem(itemId, item.Quantity - 1);
                }
            }

            BindCart();
        }

        protected void btnCheckout_Click(object sender, EventArgs e)
        {
            if (!User.Identity.IsAuthenticated)
            {
                Response.Redirect("~/Account/Login.aspx?ReturnUrl=%2fCheckout%2fCheckout.aspx");
                return;
            }
            Response.Redirect("~/Checkout/Checkout.aspx");
        }

        private string GetBuyerId()
        {
            return BuyerIdAccessor.Get(this);
        }
    }
}
