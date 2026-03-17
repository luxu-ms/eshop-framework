using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
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

        public async Task<Order> CreateOrderFromBasketAsync(
            string buyerId,
            string buyerName,
            Address shippingAddress,
            string cardNumber,
            string cardHolderName,
            DateTime cardExpiration,
            string cardSecurityNumber,
            int cardTypeId)
        {
            var basket = await _context.Baskets
                .Include(b => b.Items)
                .FirstOrDefaultAsync(b => b.BuyerId == buyerId);

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

            await _context.SaveChangesAsync();

            return order;
        }

        public async Task<List<Order>> GetOrdersForBuyerAsync(string buyerId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.BuyerId == buyerId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderAsync(int orderId, string buyerId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.BuyerId == buyerId);
        }

        public async Task<Order?> GetOrderAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task UpdateOrderStatusAsync(int orderId, OrderStatus status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.Status = status;
                await _context.SaveChangesAsync();
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
