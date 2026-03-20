using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using eShopLegacy.Models;

namespace eShopLegacy.DAL
{
    public class BasketService
    {
        private readonly eShopContext _context;

        public BasketService(eShopContext context)
        {
            _context = context;
        }

        public async Task<Basket> GetOrCreateBasketAsync(string buyerId)
        {
            var basket = await _context.Baskets
                .Include(b => b.Items)
                .FirstOrDefaultAsync(b => b.BuyerId == buyerId);

            if (basket == null)
            {
                basket = new Basket { BuyerId = buyerId };
                _context.Baskets.Add(basket);
                await _context.SaveChangesAsync();
            }

            return basket;
        }

        public async Task<Basket> GetBasketAsync(string buyerId)
        {
            return await _context.Baskets
                .Include(b => b.Items)
                .FirstOrDefaultAsync(b => b.BuyerId == buyerId);
        }

        public async Task AddItemToBasketAsync(string buyerId, int catalogItemId, decimal price, int quantity = 1)
        {
            var basket = await GetOrCreateBasketAsync(buyerId);

            var catalogItem = await _context.CatalogItems.FindAsync(catalogItemId);
            if (catalogItem == null) return;

            var existingItem = basket.Items.FirstOrDefault(i => i.CatalogItemId == catalogItemId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                basket.Items.Add(new BasketItem
                {
                    BasketId      = basket.Id,
                    CatalogItemId = catalogItemId,
                    ProductName   = catalogItem.Name,
                    UnitPrice     = price,
                    OldUnitPrice  = price,
                    Quantity      = quantity,
                    PictureUrl    = catalogItem.PictureUri
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpdateBasketItemAsync(int basketItemId, int quantity)
        {
            var item = await _context.BasketItems.FindAsync(basketItemId);
            if (item == null) return;

            if (quantity <= 0)
                _context.BasketItems.Remove(item);
            else
                item.Quantity = quantity;

            await _context.SaveChangesAsync();
        }

        public async Task RemoveItemFromBasketAsync(int basketItemId)
        {
            var item = await _context.BasketItems.FindAsync(basketItemId);
            if (item != null)
            {
                _context.BasketItems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ClearBasketAsync(string buyerId)
        {
            var basket = await GetBasketAsync(buyerId);
            if (basket == null) return;

            _context.BasketItems.RemoveRange(basket.Items);
            await _context.SaveChangesAsync();
        }


        public async Task<int> GetBasketItemCountAsync(string buyerId)
        {
            var basket = await GetBasketAsync(buyerId);
            return basket?.Items.Sum(i => i.Quantity) ?? 0;
        }

        public async Task<decimal> GetBasketTotalAsync(string buyerId)
        {
            var basket = await GetBasketAsync(buyerId);
            return basket?.Items.Sum(i => i.UnitPrice * i.Quantity) ?? 0;
        }
    }
}
