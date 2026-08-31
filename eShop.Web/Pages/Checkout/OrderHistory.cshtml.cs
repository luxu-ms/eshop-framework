using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using eShopLegacy.DAL;
using eShopLegacy.Models;
namespace eShop.Web.Pages.Checkout { [Authorize] public class OrderHistoryModel:PageModel { private readonly OrderService _orders; public OrderHistoryModel(OrderService orders){_orders=orders;} public List<Order> Orders{get;private set;} public void OnGet(){Orders=_orders.GetOrdersForBuyer(User.Identity.Name);} } }