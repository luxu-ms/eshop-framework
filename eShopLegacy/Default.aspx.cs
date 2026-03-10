using System;
using System.Web.UI;

namespace eShopLegacy
{
    public partial class Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Redirect("~/Catalog/Default.aspx");
        }
    }
}
