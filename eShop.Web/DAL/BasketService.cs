using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using eShopLegacy.Models;

namespace eShopLegacy.DAL
{
    public class BasketService
    {
        private readonly CommerceContext _context;

        public BasketService(CommerceContext context)
        {
            _context = context;
        }

        public Basket GetOrCreateBasket(string buyerId)
        {
            var basket = _context.Baskets.Include(b => b.Items).FirstOrDefault(b => b.BuyerId == buyerId);
            if (basket == null)
            {
                basket = new Basket { BuyerId = buyerId };
                _context.Baskets.Add(basket);
                _context.SaveChanges();
            }

            return basket;
        }

        public Basket GetBasket(string buyerId)
        {
            return _context.Baskets.Include(b => b.Items).FirstOrDefault(b => b.BuyerId == buyerId);
        }

        public void AddItemToBasket(string buyerId, int catalogItemId, decimal price, int quantity = 1)
        {
            var basket = GetOrCreateBasket(buyerId);
            var catalogItem = _context.CatalogItems.Find(catalogItemId);
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
                    BasketId = basket.Id,
                    CatalogItemId = catalogItemId,
                    ProductName = catalogItem.Name,
                    UnitPrice = price,
                    OldUnitPrice = price,
                    Quantity = quantity,
                    PictureUrl = catalogItem.PictureUri
                });
            }

            _context.SaveChanges();
        }

        public void UpdateBasketItem(int basketItemId, int quantity)
        {
            var item = _context.BasketItems.Find(basketItemId);
            if (item == null) return;

            if (quantity <= 0)
                _context.BasketItems.Remove(item);
            else
                item.Quantity = quantity;

            _context.SaveChanges();
        }

        public void RemoveItemFromBasket(int basketItemId)
        {
            var item = _context.BasketItems.Find(basketItemId);
            if (item != null)
            {
                _context.BasketItems.Remove(item);
                _context.SaveChanges();
            }
        }

        public void ClearBasket(string buyerId)
        {
            var basket = GetBasket(buyerId);
            if (basket == null) return;

            _context.BasketItems.RemoveRange(basket.Items);
            _context.SaveChanges();
        }

        public int GetBasketItemCount(string buyerId)
        {
            var basket = GetBasket(buyerId);
            return basket?.Items.Sum(i => i.Quantity) ?? 0;
        }

        public decimal GetBasketTotal(string buyerId)
        {
            var basket = GetBasket(buyerId);
            return basket?.Items.Sum(i => i.UnitPrice * i.Quantity) ?? 0;
        }
    }
}