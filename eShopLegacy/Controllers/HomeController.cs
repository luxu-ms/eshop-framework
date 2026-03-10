using System.Web.Mvc;

namespace eShopLegacy.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Catalog");
        }
    }
}
