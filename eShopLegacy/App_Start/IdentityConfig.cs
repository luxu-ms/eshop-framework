// IdentityConfig.cs
// The local ASP.NET Identity UserManager has been replaced by Microsoft Entra ID authentication.
// User authentication is now handled via OWIN OpenIdConnect middleware configured in Startup.Auth.cs.
// User identities are managed in Microsoft Entra ID (Azure Active Directory).
// See App_Start/Startup.Auth.cs for the OIDC authentication configuration.

namespace eShopLegacy.App_Start
{
    // Retained as empty placeholder to avoid breaking any references.
    // All identity functionality is now provided by Microsoft Entra ID.
    public static class IdentityConfig
    {
    }
}
