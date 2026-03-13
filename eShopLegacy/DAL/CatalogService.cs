using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using eShopLegacy.Models;

namespace eShopLegacy.DAL
{
    public class CatalogService
    {
        private readonly eShopContext _context;

        public CatalogService(eShopContext context)
        {
            _context = context;
        }

        // ── Catalog Items ──────────────────────────────────────────

        public async Task<(List<CatalogItem> Items, int TotalItems)> GetCatalogItemsAsync(
            int pageIndex, int pageSize,
            int? brandId, int? typeId,
            string? searchText)
        {
            IQueryable<CatalogItem> query = _context.CatalogItems
                .Include(c => c.CatalogBrand)
                .Include(c => c.CatalogType);

            if (brandId.HasValue && brandId > 0)
                query = query.Where(c => c.CatalogBrandId == brandId.Value);

            if (typeId.HasValue && typeId > 0)
                query = query.Where(c => c.CatalogTypeId == typeId.Value);

            if (!string.IsNullOrEmpty(searchText))
                query = query.Where(c => c.Name.Contains(searchText) || c.Description.Contains(searchText));

            int totalItems = await query.CountAsync();

            var items = await query
                .OrderBy(c => c.Name)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalItems);
        }

        public async Task<CatalogItem?> GetCatalogItemAsync(int id)
        {
            return await _context.CatalogItems
                .Include(c => c.CatalogBrand)
                .Include(c => c.CatalogType)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<CatalogItem>> GetCatalogItemsByNameAsync(string name)
        {
            return await _context.CatalogItems
                .Include(c => c.CatalogBrand)
                .Include(c => c.CatalogType)
                .Where(c => c.Name.Contains(name))
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task AddCatalogItemAsync(CatalogItem item)
        {
            _context.CatalogItems.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCatalogItemAsync(CatalogItem item)
        {
            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCatalogItemAsync(int id)
        {
            var item = await _context.CatalogItems.FindAsync(id);
            if (item != null)
            {
                _context.CatalogItems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        // ── Brands & Types ─────────────────────────────────────────

        public async Task<List<CatalogBrand>> GetCatalogBrandsAsync()
        {
            return await _context.CatalogBrands.OrderBy(b => b.Brand).ToListAsync();
        }

        public async Task<List<CatalogType>> GetCatalogTypesAsync()
        {
            return await _context.CatalogTypes.OrderBy(t => t.Type).ToListAsync();
        }
    }
}
