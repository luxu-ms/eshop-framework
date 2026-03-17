using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eShopLegacy.Models
{
    public enum OrderStatus
    {
        Submitted = 1,
        AwaitingValidation = 2,
        StockConfirmed = 3,
        Paid = 4,
        Shipped = 5,
        Cancelled = 6
    }

    public class Order
    {
        public int Id { get; set; }

        public string BuyerId { get; set; }

        public string BuyerName { get; set; }

        public DateTime OrderDate { get; set; }

        public OrderStatus Status { get; set; }

        // Shipping address (flattened)
        [StringLength(180)] public string Street { get; set; }
        [StringLength(100)] public string City { get; set; }
        [StringLength(60)]  public string State { get; set; }
        [StringLength(200)] public string Country { get; set; }
        [StringLength(18)]  public string ZipCode { get; set; }

        // Payment info (card number masked)
        [StringLength(25)]  public string CardNumber { get; set; }
        [StringLength(50)]  public string CardHolderName { get; set; }
        public DateTime     CardExpiration { get; set; }
        [StringLength(4)]   public string? CardSecurityNumber { get; set; }
        public int          CardTypeId { get; set; }

        public decimal Total { get; set; }

        public string? Description { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
