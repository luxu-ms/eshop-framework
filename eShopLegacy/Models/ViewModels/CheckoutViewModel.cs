using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using eShopLegacy.Models;

namespace eShopLegacy.Models.ViewModels
{
    public class CheckoutViewModel
    {
        // Shipping address
        [Required]
        public string Street { get; set; }

        [Required]
        public string City { get; set; }

        public string State { get; set; }

        [Required]
        [Display(Name = "ZIP Code")]
        public string ZipCode { get; set; }

        [Required]
        public string Country { get; set; }

        // Payment
        [Required]
        [Display(Name = "Card Number")]
        [RegularExpression(@"^\d{13,19}$", ErrorMessage = "Enter a valid card number (13-19 digits).")]
        public string CardNumber { get; set; }

        [Required]
        [Display(Name = "Card Holder Name")]
        public string CardHolderName { get; set; }

        [Required]
        [Display(Name = "Expiry (MM/YY)")]
        [RegularExpression(@"^\d{2}/\d{2}$", ErrorMessage = "Use MM/YY format.")]
        public string CardExpiry { get; set; }

        [Required]
        [Display(Name = "CVV")]
        [RegularExpression(@"^\d{3,4}$", ErrorMessage = "Enter a valid CVV.")]
        public string CardSecurityNumber { get; set; }

        // Order summary (read-only, not posted)
        public List<BasketItem> OrderItems { get; set; } = new List<BasketItem>();
        public decimal OrderTotal => OrderItems?.Sum(i => i.UnitPrice * i.Quantity) ?? 0;
    }
}
