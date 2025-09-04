
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using RMS.Data.Entities;
using RMS.Data.Services;
using RMS.Data.Repository;
using RMS.Data.Seeders;
using RMS.Models;
using RMS.Web.Services;

 using Microsoft.Extensions.DependencyInjection;
using System.Net.Mail;
using System.Net;


var builder = WebApplication.CreateBuilder(args);

// Register DbContext BEFORE building the app
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlite("Data Source=rms.db"));

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add cookie authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

// Register application services
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IUserService, UserServiceDb>();
builder.Services.AddScoped<IPaymentMethodService, PaymentMethodService>();
builder.Services.AddScoped<IDeliveryAddressService, DeliveryAddressService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<DataSeeder>();
builder.Services.AddSingleton<ProductRecommendationService>();

builder.Services.AddHttpClient();

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

    builder.Services.AddScoped<IUserService, UserServiceDb>();


var app = builder.Build();

// Seed data in development mode
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

    // Seed users
    var userService = services.GetRequiredService<IUserService>();
    ServiceSeeder.SeedUsers(userService);

    // Seed products (optional)
    var productService = services.GetRequiredService<IProductService>();
    ServiceSeeder.SeedProducts(productService);

    // Seed categories using DataSeeder
    var dataSeeder = services.GetRequiredService<DataSeeder>();
    await dataSeeder.SeedAsync();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login"; // or whatever your login route is
});

public partial class Program { }




