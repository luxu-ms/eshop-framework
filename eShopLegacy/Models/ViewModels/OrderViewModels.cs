using System.Collections.Generic;
using eShopLegacy.Models;

namespace eShopLegacy.Models.ViewModels
{
    public class OrderCompleteViewModel
    {
        public int OrderId { get; set; }
        public string OrderDate { get; set; }
        public decimal Total { get; set; }
        public ICollection<OrderItem> Items { get; set; }
    }

    public class OrderHistoryViewModel
    {
        public List<Order> Orders { get; set; } = new List<Order>();
    }
}
