using System.Collections.Generic;
using eShopLegacy.Models;

namespace eShopLegacy.Models.ViewModels
{
    public class CatalogIndexViewModel
    {
        public List<CatalogItem> Items { get; set; }
        public List<CatalogBrand> Brands { get; set; }
        public List<CatalogType> Types { get; set; }

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }

        public int? SelectedBrandId { get; set; }
        public int? SelectedTypeId { get; set; }
        public string SearchText { get; set; }

        public bool HasPreviousPage => CurrentPage > 0;
        public bool HasNextPage => (CurrentPage + 1) * PageSize < TotalItems;
        public int PageSize { get; set; } = 8;
    }
}
