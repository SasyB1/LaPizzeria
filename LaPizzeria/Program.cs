using LaPizzeria;
using LaPizzeria.Models;
using LaPizzeria.Services.Interfaces;
using LaPizzeria.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();


builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();


builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Home/Index";
    });

builder.Services
    .AddAuthorization(options =>
    {
        options.AddPolicy(Policies.IsAdmin, policy => policy.RequireRole("admin"));
        options.AddPolicy(Policies.IsUser, policy => policy.RequireRole("user"));
        options.AddPolicy(Policies.SupplierOrCustomer, policy =>
            policy.RequireAssertion(context =>
                context.User.HasClaim(c =>
                    (c.Type == ClaimTypes.Role && c.Value == "user") ||
                    (c.Type == ClaimTypes.Role && c.Value == "admin"))));
    });


builder.Services.AddDbContext<InFornoDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services
    .AddScoped<IAuthService, AuthService>()
    .AddScoped<IIngredientService, IngredientService>()
    .AddScoped<ICartService, CartService>()
    .AddScoped<IProductService, ProductService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
