using Microsoft.AspNetCore.Identity;

namespace eShopLegacy.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? Name { get; set; }

        public string? LastName { get; set; }

        public string? Street { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        public string? Country { get; set; }

        public string? ZipCode { get; set; }

        public int CardTypeId { get; set; }

        public string? CardNumber { get; set; }

        public string? CardHolderName { get; set; }

        public string? CardExpiration { get; set; }
    }
}
