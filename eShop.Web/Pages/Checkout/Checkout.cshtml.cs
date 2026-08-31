using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using eShopLegacy.DAL;
using eShopLegacy.Models;

namespace eShop.Web.Pages.Checkout
{
    [Authorize]
    public class CheckoutModel : PageModel
    {
        private readonly CommerceContext _context; private readonly BasketService _basket; private readonly OrderService _orders;
        public CheckoutModel(CommerceContext context, BasketService basket, OrderService orders) { _context=context; _basket=basket; _orders=orders; }
        [BindProperty, Required] public string Street { get; set; } [BindProperty, Required] public string City { get; set; } [BindProperty, Required] public string State { get; set; } [BindProperty, Required] public string Country { get; set; } [BindProperty, Required] public string ZipCode { get; set; }
        [BindProperty, Required, CreditCard] public string CardNumber { get; set; } [BindProperty, Required, Display(Name="Card holder name")] public string CardHolderName { get; set; } [BindProperty, Required, Display(Name="Expiration")] public string CardExpiration { get; set; } [BindProperty, Required, RegularExpression("[0-9]{3}"), Display(Name="Security number")] public string CardSecurityNumber { get; set; }
        public IActionResult OnGet()
        {
            if (_basket.GetBasket(User.Identity.Name)?.Items.Any() != true) return Redirect("/Cart/ShoppingCart.aspx");
            var user=_context.Users.FirstOrDefault(x=>x.UserName==User.Identity.Name); if(user!=null){Street=user.Street;City=user.City;State=user.State;Country=user.Country;ZipCode=user.ZipCode;} return Page();
        }
        public IActionResult OnPost()
        {
            if(!DateTime.TryParseExact(CardExpiration+"-01","yyyy-MM-dd",CultureInfo.InvariantCulture,DateTimeStyles.None,out var expiry)) ModelState.AddModelError(nameof(CardExpiration),"Invalid expiration.");
            if(!ModelState.IsValid) return Page();
            var order=_orders.CreateOrderFromBasket(User.Identity.Name,User.Identity.Name,new Address{Street=Street,City=City,State=State,Country=Country,ZipCode=ZipCode},CardNumber,CardHolderName,expiry,CardSecurityNumber,1);
            return Redirect("/Checkout/OrderComplete.aspx?orderId="+order.Id);
        }
    }
}