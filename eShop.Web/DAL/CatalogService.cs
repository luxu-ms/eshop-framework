using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using eShopLegacy.Models;

namespace eShopLegacy.DAL
{
    public class CatalogService
    {
        private readonly CommerceContext _context;

        public CatalogService(CommerceContext context)
        {
            _context = context;
        }

        public List<CatalogItem> GetCatalogItems(int pageIndex, int pageSize, int? brandId, int? typeId, string searchText, out int totalItems)
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

            totalItems = query.Count();

            return query.OrderBy(c => c.Name)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public CatalogItem GetCatalogItem(int id)
        {
            return _context.CatalogItems
                .Include(c => c.CatalogBrand)
                .Include(c => c.CatalogType)
                .FirstOrDefault(c => c.Id == id);
        }

        public List<CatalogItem> GetCatalogItemsByName(string name)
        {
            return _context.CatalogItems
                .Include(c => c.CatalogBrand)
                .Include(c => c.CatalogType)
                .Where(c => c.Name.Contains(name))
                .OrderBy(c => c.Name)
                .ToList();
        }

        public void AddCatalogItem(CatalogItem item)
        {
            _context.CatalogItems.Add(item);
            _context.SaveChanges();
        }

        public void UpdateCatalogItem(CatalogItem item)
        {
            _context.Entry(item).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void DeleteCatalogItem(int id)
        {
            var item = _context.CatalogItems.Find(id);
            if (item != null)
            {
                _context.CatalogItems.Remove(item);
                _context.SaveChanges();
            }
        }

        public List<CatalogBrand> GetCatalogBrands()
        {
            return _context.CatalogBrands.OrderBy(b => b.Brand).ToList();
        }

        public List<CatalogType> GetCatalogTypes()
        {
            return _context.CatalogTypes.OrderBy(t => t.Type).ToList();
        }
    }
}