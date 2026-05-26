using Microsoft.AspNet.Identity.EntityFramework;

namespace eShopLegacy.Models
{
    /// <summary>
    /// Application user entity for EF6 Identity schema.
    /// Authentication is handled by Microsoft Entra ID (OIDC) – not local credentials.
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
