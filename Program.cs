using JanSamadhan.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using JanSamadhan.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.AutoValidateAntiforgeryTokenAttribute());
});

builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=JanSamadhan.db";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

builder.Services.AddScoped<IPasswordService, PasswordService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Ensure database is available on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    var passwordService = scope.ServiceProvider.GetRequiredService<IPasswordService>();
    try
    {
        db.Database.EnsureCreated();
    }
    catch { }

    // Seed a single admin account from environment/configuration if none exists
    try
    {
        if (!db.Admins.Any())
        {
            var adminEmail = Environment.GetEnvironmentVariable("ADMIN_EMAIL") ?? config["Admin:Email"];
            var adminPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD") ?? config["Admin:Password"];
            var adminName = Environment.GetEnvironmentVariable("ADMIN_NAME") ?? config["Admin:Name"] ?? "Administrator";

            if (!string.IsNullOrWhiteSpace(adminEmail) && !string.IsNullOrWhiteSpace(adminPassword) && adminPassword != "Admin@1")
            {
                db.Admins.Add(new JanSamadhan.Models.Admin
                {
                    Name = adminName,
                    Email = adminEmail,
                    PasswordHash = passwordService.HashPassword(adminPassword)
                });
                db.SaveChanges();
            }
        }
    }
    catch { }
}

app.Run();
