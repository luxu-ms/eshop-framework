using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using eShopLegacy.DAL;
using eShopLegacy.Models;

namespace eShopLegacy.Admin
{
    public partial class ProductsAdminPage : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!User.Identity.IsAuthenticated)
                Response.Redirect("~/Account/Login.aspx");

            if (!IsPostBack)
            {
                BindDropDowns();
                BindGrid();
            }
        }

        private void BindDropDowns()
        {
            using (var ctx = new eShopContext())
            {
                var svc = new CatalogService(ctx);

                ddlBrand.DataSource     = svc.GetCatalogBrands();
                ddlBrand.DataTextField  = "Brand";
                ddlBrand.DataValueField = "Id";
                ddlBrand.DataBind();

                ddlType.DataSource     = svc.GetCatalogTypes();
                ddlType.DataTextField  = "Type";
                ddlType.DataValueField = "Id";
                ddlType.DataBind();
            }
        }

        private void BindGrid()
        {
            using (var ctx = new eShopContext())
            {
                int total;
                var items = new CatalogService(ctx).GetCatalogItems(0, int.MaxValue, null, null, string.Empty, out total);
                gvProducts.DataSource = items;
                gvProducts.DataBind();
            }
        }

        protected void btnShowAdd_Click(object sender, EventArgs e)
        {
            hfEditId.Value    = "0";
            lblFormTitle.Text = "Add Product";
            txtName.Text      = txtPrice.Text = txtDescription.Text = txtStock.Text = "";
            pnlForm.Visible   = true;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            pnlForm.Visible = false;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            int editId = int.Parse(hfEditId.Value);
            decimal price; decimal.TryParse(txtPrice.Text, out price);
            int stock;    int.TryParse(txtStock.Text, out stock);

            using (var ctx = new eShopContext())
            {
                var svc = new CatalogService(ctx);
                if (editId == 0)
                {
                    svc.AddCatalogItem(new CatalogItem
                    {
                        Name           = txtName.Text.Trim(),
                        Price          = price,
                        Description    = txtDescription.Text.Trim(),
                        CatalogBrandId = int.Parse(ddlBrand.SelectedValue),
                        CatalogTypeId  = int.Parse(ddlType.SelectedValue),
                        AvailableStock = stock
                    });
                    ShowSuccess("Product added successfully.");
                }
                else
                {
                    var item = ctx.CatalogItems.Find(editId);
                    if (item != null)
                    {
                        item.Name           = txtName.Text.Trim();
                        item.Price          = price;
                        item.Description    = txtDescription.Text.Trim();
                        item.CatalogBrandId = int.Parse(ddlBrand.SelectedValue);
                        item.CatalogTypeId  = int.Parse(ddlType.SelectedValue);
                        item.AvailableStock = stock;
                        svc.UpdateCatalogItem(item);
                    }
                    ShowSuccess("Product updated successfully.");
                }
            }

            pnlForm.Visible = false;
            BindGrid();
        }

        protected void gvProducts_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int itemId = int.Parse(e.CommandArgument.ToString());

            if (e.CommandName == "DeleteItem")
            {
                using (var ctx = new eShopContext())
                    new CatalogService(ctx).DeleteCatalogItem(itemId);
                ShowSuccess("Product deleted.");
                BindGrid();
            }
            else if (e.CommandName == "EditItem")
            {
                using (var ctx = new eShopContext())
                {
                    var item = new CatalogService(ctx).GetCatalogItem(itemId);
                    if (item == null) return;

                    hfEditId.Value    = item.Id.ToString();
                    lblFormTitle.Text = "Edit Product";
                    txtName.Text      = item.Name;
                    txtPrice.Text     = item.Price.ToString("0.00");
                    txtDescription.Text = item.Description;
                    txtStock.Text     = item.AvailableStock.ToString();
                    ddlBrand.SelectedValue = item.CatalogBrandId.ToString();
                    ddlType.SelectedValue  = item.CatalogTypeId.ToString();
                }
                pnlForm.Visible = true;
            }
        }

        private void ShowSuccess(string msg)
        {
            pnlSuccess.Visible   = true;
            litSuccessMsg.Text   = msg;
        }
    }
}
