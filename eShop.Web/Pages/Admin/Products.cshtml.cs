using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using eShopLegacy.DAL;
using eShopLegacy.Models;

namespace eShop.Web.Pages.Admin
{
    [Authorize]
    public class ProductsModel : PageModel
    {
        private readonly CatalogService _catalog; private readonly CommerceContext _context;
        public ProductsModel(CatalogService catalog,CommerceContext context){_catalog=catalog;_context=context;}
        public List<CatalogItem> Items{get;private set;} public List<SelectListItem> Brands{get;private set;} public List<SelectListItem> Types{get;private set;} public string Message{get;private set;}
        [BindProperty(SupportsGet=true)] public int Id{get;set;} [BindProperty,Required] public string Name{get;set;} [BindProperty] public decimal Price{get;set;} [BindProperty] public int Stock{get;set;} [BindProperty] public string Description{get;set;} [BindProperty] public int BrandId{get;set;} [BindProperty] public int TypeId{get;set;}
        public void OnGet(){if(Id>0){var x=_catalog.GetCatalogItem(Id);if(x!=null){Name=x.Name;Price=x.Price;Stock=x.AvailableStock;Description=x.Description;BrandId=x.CatalogBrandId;TypeId=x.CatalogTypeId;}}Load();}
        public IActionResult OnPostSave(){if(!ModelState.IsValid){Load();return Page();}if(Id==0)_catalog.AddCatalogItem(new CatalogItem{Name=Name.Trim(),Price=Price,AvailableStock=Stock,Description=Description?.Trim(),CatalogBrandId=BrandId,CatalogTypeId=TypeId});else{var x=_context.CatalogItems.Find(Id);if(x!=null){x.Name=Name.Trim();x.Price=Price;x.AvailableStock=Stock;x.Description=Description?.Trim();x.CatalogBrandId=BrandId;x.CatalogTypeId=TypeId;_catalog.UpdateCatalogItem(x);}}return Redirect("/Admin/Products.aspx");}
        public IActionResult OnPostDelete(int id){_catalog.DeleteCatalogItem(id);return Redirect("/Admin/Products.aspx");}
        private void Load(){Items=_catalog.GetCatalogItems(0,int.MaxValue,null,null,"",out _);Brands=_catalog.GetCatalogBrands().ConvertAll(x=>new SelectListItem(x.Brand,x.Id.ToString(),x.Id==BrandId));Types=_catalog.GetCatalogTypes().ConvertAll(x=>new SelectListItem(x.Type,x.Id.ToString(),x.Id==TypeId));}
    }
}