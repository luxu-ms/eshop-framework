using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using eShopLegacy.DAL;
using eShopLegacy.Models;

namespace eShopLegacy.App_Start
{
    public class IdentityConfig
    {
        public static UserManager<ApplicationUser> CreateUserManager()
        {
            var manager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(new eShopContext()));

            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false
            };

            return manager;
        }
    }
}
