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
        private static string clientId     = ConfigurationManager.AppSettings["ida:ClientId"];
        private static string aadInstance  = ConfigurationManager.AppSettings["ida:AADInstance"];
        private static string tenantId     = ConfigurationManager.AppSettings["ida:TenantId"];
        private static string postLogoutRedirectUri = ConfigurationManager.AppSettings["ida:PostLogoutRedirectUri"];

        public void ConfigureAuth(IAppBuilder app)
        {
            string authority = string.Format(aadInstance ?? "https://login.microsoftonline.com/{0}", tenantId);

            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = CookieAuthenticationDefaults.AuthenticationType,
                LoginPath          = new PathString("/Account/Login.aspx")
            });

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                ClientId    = clientId,
                Authority   = authority,
                RedirectUri = postLogoutRedirectUri,
                PostLogoutRedirectUri = postLogoutRedirectUri,
                Scope       = "openid profile email",
                ResponseType = "id_token",
                TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = false,
                    NameClaimType  = "name"
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
