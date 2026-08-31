using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using eShopLegacy.DAL;
using eShop.Web.Services;
using eShopLegacy.Models;

var builder = WebApplication.CreateBuilder(args);
var configuredDataDirectory = builder.Configuration["DataDirectory"];
var dataDirectory = Path.GetFullPath(configuredDataDirectory ?? Path.Combine(builder.Environment.ContentRootPath, "..", "eShopLegacy", "App_Data"));
AppDomain.CurrentDomain.SetData("DataDirectory", dataDirectory);
Database.SetInitializer<CommerceContext>(null);
var commerceConnectionString = builder.Configuration
	.GetConnectionString("eShopContext") ?? throw new InvalidOperationException("ConnectionStrings:eShopContext is required.");
commerceConnectionString = commerceConnectionString
	.Replace("|DataDirectory|", dataDirectory);

builder.Services.AddScoped(_ => new CommerceContext(commerceConnectionString));
builder.Services.AddScoped<CatalogService>();
builder.Services.AddScoped<BasketService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IBuyerIdAccessor, BuyerIdAccessor>();
builder.Services.Configure<PasswordHasherOptions>(options => options.CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV2);
builder.Services.AddScoped<IPasswordHasher<UserRecord>, PasswordHasher<UserRecord>>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options =>
	{
		options.LoginPath = "/Account/Login.aspx";
		options.ExpireTimeSpan = TimeSpan.FromHours(48);
		options.SlidingExpiration = true;
	});
builder.Services.AddAuthorization();
builder.Services.AddRazorPages();

var app = builder.Build();

app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok(new { status = "Healthy" }));
app.MapPost("/Account/Logout", async (HttpContext context) =>
{
	await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
	return Results.Redirect("/");
});
app.MapGet("/health/data", (CommerceContext context) => Results.Ok(new
{
	status = "Healthy",
	catalogItems = context.CatalogItems.Count()
}));
app.MapRazorPages();

app.Run();
