using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using eShopLegacy.DAL;
using eShopLegacy.Models;

namespace eShopLegacy.Catalog
{
    public partial class CatalogPage : Page
    {
        private const int DefaultPageSize = 8;

        // All state lives in the query string — no ViewState, no postbacks for filters/paging
        private int    QsPage   => int.TryParse(Request.QueryString["page"],  out int  p) ? Math.Max(0, p) : 0;
        private int    QsBrand  => int.TryParse(Request.QueryString["brand"], out int  b) ? b : 0;
        private int    QsType   => int.TryParse(Request.QueryString["type"],  out int  t) ? t : 0;
        private string QsSearch => (Request.QueryString["q"] ?? "").Trim();

        protected void Page_Load(object sender, EventArgs e)
        {
            BindFilters();
            BindProducts();
        }

        private void BindFilters()
        {
            using (var ctx = new eShopContext())
            {
                var svc = new CatalogService(ctx);

                ddlBrand.Items.Clear();
                ddlBrand.Items.Add(new ListItem("All Brands", "0"));
                foreach (var brand in svc.GetCatalogBrands())
                    ddlBrand.Items.Add(new ListItem(brand.Brand, brand.Id.ToString()));

                ddlType.Items.Clear();
                ddlType.Items.Add(new ListItem("All Types", "0"));
                foreach (var type in svc.GetCatalogTypes())
                    ddlType.Items.Add(new ListItem(type.Type, type.Id.ToString()));
            }

            // Reflect query-string selections in the dropdowns
            var brandItem = ddlBrand.Items.FindByValue(QsBrand.ToString());
            if (brandItem != null) brandItem.Selected = true;
            var typeItem = ddlType.Items.FindByValue(QsType.ToString());
            if (typeItem != null) typeItem.Selected = true;
            txtSearch.Text = QsSearch;
        }

        private void BindProducts()
        {
            int? brandId = QsBrand == 0 ? (int?)null : QsBrand;
            int? typeId  = QsType  == 0 ? (int?)null : QsType;

            using (var ctx = new eShopContext())
            {
                var svc = new CatalogService(ctx);
                int total;
                var items = svc.GetCatalogItems(QsPage, DefaultPageSize, brandId, typeId, QsSearch, out total);

                rptProducts.DataSource = items;
                rptProducts.DataBind();

                pnlEmpty.Visible = items.Count == 0;

                int totalPages = Math.Max(1, (int)Math.Ceiling((double)total / DefaultPageSize));
                pnlPager.Visible   = total > DefaultPageSize;
                lblPage.Text       = (QsPage + 1).ToString();
                lblTotalPages.Text = totalPages.ToString();

                // Pager links — plain <a> tags, no postback
                bool hasPrev = QsPage > 0;
                bool hasNext = (QsPage + 1) * DefaultPageSize < total;
                btnPrev.NavigateUrl = hasPrev ? BuildUrl(QsPage - 1) : "#";
                btnNext.NavigateUrl = hasNext ? BuildUrl(QsPage + 1) : "#";
                btnPrev.CssClass    = "page-link" + (hasPrev ? "" : " disabled");
                btnNext.CssClass    = "page-link" + (hasNext ? "" : " disabled");
            }
        }

        // Build a query-string URL preserving current filter state
        private string BuildUrl(int page)
        {
            var url = "Default.aspx";
            var sep = "?";
            if (page > 0)              { url += sep + "page="  + page;                       sep = "&"; }
            if (QsBrand != 0)          { url += sep + "brand=" + QsBrand;                    sep = "&"; }
            if (QsType  != 0)          { url += sep + "type="  + QsType;                     sep = "&"; }
            if (!string.IsNullOrEmpty(QsSearch)) { url += sep + "q=" + Uri.EscapeDataString(QsSearch); }
            return url;
        }

        protected string GetProductImage(object pictureUri)
        {
            var uri = pictureUri?.ToString();
            if (string.IsNullOrEmpty(uri)) return ResolveUrl("~/Content/placeholder.png");
            return ResolveUrl("~/Content/placeholder.png"); // Replace with actual image path
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            // Build redirect URL with the typed search term
            var q   = txtSearch.Text.Trim();
            var url = "Default.aspx";
            var sep = "?";
            if (QsBrand != 0)          { url += sep + "brand=" + QsBrand;             sep = "&"; }
            if (QsType  != 0)          { url += sep + "type="  + QsType;              sep = "&"; }
            if (!string.IsNullOrEmpty(q)) { url += sep + "q=" + Uri.EscapeDataString(q); }
            Response.Redirect(url);
        }

        protected void btnClearFilters_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx");
        }

        protected void btnAddToCart_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandName != "AddToCart") return;

            var parts     = e.CommandArgument.ToString().Split('|');
            int itemId    = int.Parse(parts[0]);
            decimal price = decimal.Parse(parts[1]);

            string buyerId = GetBuyerId();
            using (var ctx = new eShopContext())
            {
                new BasketService(ctx).AddItemToBasket(buyerId, itemId, price);
            }

            // Redirect back preserving all filters + signal the toast
            var returnUrl = BuildUrl(QsPage);
            returnUrl += (returnUrl.Contains("?") ? "&" : "?") + "added=1";
            Response.Redirect(returnUrl);
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
