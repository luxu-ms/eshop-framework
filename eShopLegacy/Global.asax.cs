using System;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using eShopLegacy.DAL;

namespace eShopLegacy
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            System.Web.UI.ValidationSettings.UnobtrusiveValidationMode = System.Web.UI.UnobtrusiveValidationMode.None;
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            DatabaseInitializer.Initialize();
        }

        void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            // Log error here
        }
    }
}
