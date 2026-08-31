using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eShop.Web.Pages
{
    public class IndexModel : PageModel
    {
        public IActionResult OnGet()
        {
            return Redirect("/Catalog/Default.aspx");
        }
    }
}