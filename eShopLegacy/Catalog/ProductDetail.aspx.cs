using System;
using System.Web.UI;
using eShopLegacy.DAL;
using eShopLegacy.Models;

namespace eShopLegacy.Catalog
{
    public partial class ProductDetailPage : Page
    {
        private int _productId;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!int.TryParse(Request.QueryString["id"], out _productId))
            {
                ShowNotFound();
                return;
            }

            if (!IsPostBack) BindProduct();
        }

        private void BindProduct()
        {
            using (var ctx = new eShopContext())
            {
                var item = new CatalogService(ctx).GetCatalogItem(_productId);
                if (item == null) { ShowNotFound(); return; }

                lblBreadcrumb.Text  = item.Name;
                lblName.Text        = item.Name;
                lblBrand.Text       = item.CatalogBrand?.Brand ?? "";
                lblType.Text        = item.CatalogType?.Type  ?? "";
                lblPrice.Text       = item.Price.ToString("0.00");
                lblDescription.Text = item.Description;
                lblStock.Text       = item.AvailableStock > 0
                    ? $"In Stock ({item.AvailableStock} available)"
                    : "Out of Stock";
                if (item.AvailableStock == 0)
                    lblStock.CssClass = "badge bg-danger";

                imgProduct.Src = ResolveUrl("~/Content/placeholder.png");
                ViewState["ProductId"] = item.Id;
                ViewState["Price"]     = item.Price;
            }
        }

        protected void btnAddToCart_Click(object sender, EventArgs e)
        {
            if (ViewState["ProductId"] == null) return;

            int    id    = (int)ViewState["ProductId"];
            decimal price= (decimal)ViewState["Price"];
            int qty = 1;
            int.TryParse(txtQty.Text, out qty);
            if (qty < 1) qty = 1;

            string buyerId = GetBuyerId();
            using (var ctx = new eShopContext())
            {
                var svc = new BasketService(ctx);
                for (int i = 0; i < qty; i++)
                    svc.AddItemToBasket(buyerId, id, price);
            }

            pnlSuccess.Visible = true;
        }

        private void ShowNotFound()
        {
            pnlNotFound.Visible = true;
            pnlProduct.Visible  = false;
        }

        private string GetBuyerId()
        {
            if (User.Identity.IsAuthenticated) return User.Identity.Name;
            if (Session["AnonymousBuyerId"] == null)
                Session["AnonymousBuyerId"] = Guid.NewGuid().ToString();
            return Session["AnonymousBuyerId"].ToString();
        }
    }
}
