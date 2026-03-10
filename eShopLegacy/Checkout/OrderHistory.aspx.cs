using System;
using System.Web.UI;
using eShopLegacy.DAL;
using eShopLegacy.Models;

namespace eShopLegacy.Checkout
{
    public partial class OrderHistoryPage : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!User.Identity.IsAuthenticated)
            {
                Response.Redirect("~/Account/Login.aspx?returnUrl=~/Checkout/OrderHistory.aspx");
                return;
            }

            if (!IsPostBack)
                BindOrders();
        }

        private void BindOrders()
        {
            using (var ctx = new eShopContext())
            {
                var orders = new OrderService(ctx).GetOrdersForBuyer(User.Identity.Name);
                pnlEmpty.Visible = orders.Count == 0;
                rptOrders.DataSource = orders;
                rptOrders.DataBind();
            }
        }

        protected string GetStatusBadge(object status)
        {
            switch ((OrderStatus)status)
            {
                case OrderStatus.Paid:
                case OrderStatus.Shipped:    return "bg-success";
                case OrderStatus.Cancelled:  return "bg-danger";
                case OrderStatus.Submitted:  return "bg-primary";
                default:                     return "bg-secondary";
            }
        }
    }
}
