// IdentityConfig.cs
// NOTE: Local ASP.NET Identity user management has been superseded by Microsoft Entra ID
// authentication (see App_Start/Startup.Auth.cs).
// The eShopContext still extends IdentityDbContext<ApplicationUser> to maintain the
// existing database schema for user profile data (address, card info, etc.),
// but authentication is now handled entirely by Azure AD / Entra ID via OIDC.
//
// This file is retained as a no-op placeholder.  If local user profile seeding or
// administrative user creation is needed in future, re-implement here using
// Microsoft Graph API or Azure AD B2C custom policies.
namespace eShopLegacy.App_Start
{
    public static class IdentityConfig
    {
        // No local user manager — authentication is delegated to Microsoft Entra ID.
    }
}
