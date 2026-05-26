using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace eShopLegacy
{
    public partial class Startup
    {
        private static readonly string _clientId =
            ConfigurationManager.AppSettings["ida:ClientId"];
        private static readonly string _aadInstance =
            ConfigurationManager.AppSettings["ida:AADInstance"];
        private static readonly string _tenantId =
            ConfigurationManager.AppSettings["ida:TenantId"];
        private static readonly string _postLogoutRedirectUri =
            ConfigurationManager.AppSettings["ida:PostLogoutRedirectUri"];
        private static readonly string _authority =
            string.Format(_aadInstance, _tenantId);

        public void ConfigureAuth(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = CookieAuthenticationDefaults.AuthenticationType,
                LoginPath = new PathString("/Account/Login.aspx"),
                SlidingExpiration = true,
                ExpireTimeSpan = TimeSpan.FromHours(8)
            });

            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    ClientId = _clientId,
                    Authority = _authority,
                    PostLogoutRedirectUri = _postLogoutRedirectUri,
                    RedirectUri = _postLogoutRedirectUri,
                    Scope = "openid profile email",
                    ResponseType = "id_token",
                    TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        NameClaimType = "name"
                    },
                    Notifications = new OpenIdConnectAuthenticationNotifications
                    {
                        AuthenticationFailed = context =>
                        {
                            context.HandleResponse();
                            context.Response.Redirect("/Error?message=authentication_failed");
                            return Task.FromResult(0);
                        }
                    }
                });
        }
    }
}
