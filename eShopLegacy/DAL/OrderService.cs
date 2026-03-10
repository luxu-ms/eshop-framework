using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using eShopLegacy.Models;

namespace eShopLegacy.DAL
{
    public class OrderService
    {
        private readonly eShopContext _context;

        public OrderService(eShopContext context)
        {
            _context = context;
        }

        public Order CreateOrderFromBasket(
            string buyerId,
            string buyerName,
            Address shippingAddress,
            string cardNumber,
            string cardHolderName,
            DateTime cardExpiration,
            string cardSecurityNumber,
            int cardTypeId)
        {
            var basket = _context.Baskets
                .Include(b => b.Items)
                .FirstOrDefault(b => b.BuyerId == buyerId);

            if (basket == null || !basket.Items.Any())
                throw new InvalidOperationException("Basket is empty.");

            var order = new Order
            {
                BuyerId           = buyerId,
                BuyerName         = buyerName,
                OrderDate         = DateTime.UtcNow,
                Status            = OrderStatus.Submitted,
                Street            = shippingAddress.Street,
                City              = shippingAddress.City,
                State             = shippingAddress.State,
                Country           = shippingAddress.Country,
                ZipCode           = shippingAddress.ZipCode,
                CardNumber        = MaskCardNumber(cardNumber),
                CardHolderName    = cardHolderName,
                CardExpiration    = cardExpiration,
                CardSecurityNumber= cardSecurityNumber,
                CardTypeId        = cardTypeId
            };

            foreach (var item in basket.Items)
            {
                order.OrderItems.Add(new OrderItem
                {
                    ProductId   = item.CatalogItemId,
                    ProductName = item.ProductName,
                    UnitPrice   = item.UnitPrice,
                    Discount    = 0,
                    Units       = item.Quantity,
                    PictureUrl  = item.PictureUrl
                });
            }

            order.Total = order.OrderItems.Sum(i => i.UnitPrice * i.Units - i.Discount);

            _context.Orders.Add(order);

            // Clear basket
            _context.BasketItems.RemoveRange(basket.Items);

            _context.SaveChanges();

            return order;
        }

        public List<Order> GetOrdersForBuyer(string buyerId)
        {
            return _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.BuyerId == buyerId)
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }

        public Order GetOrder(int orderId, string buyerId)
        {
            return _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefault(o => o.Id == orderId && o.BuyerId == buyerId);
        }

        public List<Order> GetAllOrders()
        {
            return _context.Orders
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }

        public void UpdateOrderStatus(int orderId, OrderStatus status)
        {
            var order = _context.Orders.Find(orderId);
            if (order != null)
            {
                order.Status = status;
                _context.SaveChanges();
            }
        }

        private static string MaskCardNumber(string cardNumber)
        {
            if (string.IsNullOrEmpty(cardNumber) || cardNumber.Length < 4)
                return cardNumber;
            return new string('*', cardNumber.Length - 4) + cardNumber.Substring(cardNumber.Length - 4);
        }
    }
}
