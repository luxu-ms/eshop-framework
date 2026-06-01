using Microsoft.AspNet.Identity.EntityFramework;

namespace eShopLegacy.Models
{
    /// <summary>
    /// Application user profile stored in the database.
    /// Authentication is handled by Microsoft Entra ID (Azure AD) via OIDC.
    /// This entity retains the IdentityUser base class to keep database schema compatibility
    /// with the existing AspNetUsers table, and stores extended user profile data.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }

        public string LastName { get; set; }

        public string Street { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string ZipCode { get; set; }

        public int CardTypeId { get; set; }

        public string CardNumber { get; set; }

        public string CardHolderName { get; set; }

        public string CardExpiration { get; set; }
    }
}
