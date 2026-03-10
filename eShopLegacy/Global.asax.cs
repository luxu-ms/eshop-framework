using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using eShopLegacy.DAL;

namespace eShopLegacy
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            AreaRegistration.RegisterAllAreas();
            GlobalFilters.Filters.Add(new HandleErrorAttribute());
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            DatabaseInitializer.Initialize();
        }

        void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            // Log error here
        }
    }
}
