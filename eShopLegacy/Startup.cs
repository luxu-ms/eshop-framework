using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(eShopLegacy.Startup))]

namespace eShopLegacy
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
