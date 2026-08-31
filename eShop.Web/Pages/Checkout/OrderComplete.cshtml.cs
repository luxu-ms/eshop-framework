using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using eShopLegacy.DAL;
using eShopLegacy.Models;
namespace eShop.Web.Pages.Checkout { [Authorize] public class OrderCompleteModel:PageModel { private readonly OrderService _orders; public OrderCompleteModel(OrderService orders){_orders=orders;} public Order Order{get;private set;} public void OnGet(int orderId){Order=_orders.GetOrder(orderId,User.Identity.Name);} } }