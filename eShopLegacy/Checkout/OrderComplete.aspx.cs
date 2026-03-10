using System;
using System.Web.UI;
using eShopLegacy.DAL;

namespace eShopLegacy.Checkout
{
    public partial class OrderCompletePage : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!User.Identity.IsAuthenticated)
            {
                Response.Redirect("~/");
                return;
            }

            if (!IsPostBack)
            {
                int orderId;
                if (!int.TryParse(Request.QueryString["orderId"], out orderId))
                {
                    Response.Redirect("~/");
                    return;
                }

                using (var ctx = new eShopContext())
                {
                    var order = new OrderService(ctx).GetOrder(orderId, User.Identity.Name);
                    if (order == null)
                    {
                        Response.Redirect("~/");
                        return;
                    }

                    lblOrderId.Text   = order.Id.ToString();
                    lblOrderDate.Text = order.OrderDate.ToLocalTime().ToString("f");
                    lblTotal.Text     = order.Total.ToString("0.00");

                    rptItems.DataSource = order.OrderItems;
                    rptItems.DataBind();
                }
            }
        }
    }
}
