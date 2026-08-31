using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using eShopLegacy.DAL;
using eShopLegacy.Models;

namespace eShop.Web.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly CommerceContext _context;
        private readonly IPasswordHasher<UserRecord> _hasher;
        public RegisterModel(CommerceContext context, IPasswordHasher<UserRecord> hasher) { _context = context; _hasher = hasher; }
        [BindProperty, Required, Display(Name = "First name")] public string FirstName { get; set; }
        [BindProperty, Required, Display(Name = "Last name")] public string LastName { get; set; }
        [BindProperty, Required, EmailAddress] public string Email { get; set; }
        [BindProperty, Required, MinLength(6), DataType(DataType.Password)] public string Password { get; set; }
        [BindProperty, Required, Compare(nameof(Password)), DataType(DataType.Password), Display(Name = "Confirm password")] public string ConfirmPassword { get; set; }
        public IActionResult OnGet() => User.Identity?.IsAuthenticated == true ? Redirect("/") : Page();
        public async Task<IActionResult> OnPostAsync()
        {
            if (_context.Users.Any(x => x.Email == Email)) ModelState.AddModelError(nameof(Email), "Email is already taken.");
            if (!ModelState.IsValid) return Page();
            var user = new UserRecord { Id = Guid.NewGuid().ToString(), Email = Email.Trim(), UserName = Email.Trim(), Name = FirstName.Trim(), LastName = LastName.Trim(), SecurityStamp = Guid.NewGuid().ToString(), LockoutEnabled = true };
            user.PasswordHash = _hasher.HashPassword(user, Password);
            _context.Users.Add(user); _context.SaveChanges();
            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, user.Id), new Claim(ClaimTypes.Name, user.UserName) }, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
            return Redirect("/");
        }
    }
}