using JanSamadhan.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using JanSamadhan.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=(localdb)\\mssqllocaldb;Database=JanSamadhanDb;Trusted_Connection=True;MultipleActiveResultSets=true";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
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
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Ensure database is available on startup (dev: drop and recreate to avoid drift)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    var passwordService = scope.ServiceProvider.GetRequiredService<IPasswordService>();
    // Removed automatic database deletion in Development to preserve local data across restarts
    try
    {
        db.Database.Migrate();
    }
    catch
    {
        db.Database.EnsureCreated();
    }

    // Seed a single admin account from configuration if none exists
    try
    {
        if (!db.Admins.Any())
        {
            var adminEmail = config["Admin:Email"];
            var adminPassword = config["Admin:Password"];
            var adminName = config["Admin:Name"] ?? "Administrator";

            if (!string.IsNullOrWhiteSpace(adminEmail) && !string.IsNullOrWhiteSpace(adminPassword))
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

    // Clean up unwanted users - keep only user1, user2, user3
    try
    {
        // Remove all users except user1, user2, user3
        var usersToRemove = db.Users.Where(u => !new[] { "user1", "user2", "user3" }.Contains(u.Name)).ToList();
        
        if (usersToRemove.Any())
        {
            // Remove related data first
            foreach (var user in usersToRemove)
            {
                // Remove user's complaints
                var userComplaints = db.Complaints.Where(c => c.UserId == user.UserId).ToList();
                foreach (var complaint in userComplaints)
                {
                    // Remove related data for each complaint
                    var comments = db.Comments.Where(c => c.ComplaintId == complaint.ComplaintId).ToList();
                    var ratings = db.ComplaintRatings.Where(r => r.ComplaintId == complaint.ComplaintId).ToList();
                    var assignments = db.ComplaintAssignments.Where(a => a.ComplaintId == complaint.ComplaintId).ToList();
                    
                    db.Comments.RemoveRange(comments);
                    db.ComplaintRatings.RemoveRange(ratings);
                    db.ComplaintAssignments.RemoveRange(assignments);
                }
                db.Complaints.RemoveRange(userComplaints);
                
                // Remove user's ratings
                var userRatings = db.ComplaintRatings.Where(r => r.UserId == user.UserId).ToList();
                db.ComplaintRatings.RemoveRange(userRatings);
                
                // Remove user's assignment history
                var userAssignments = db.ComplaintAssignments.Where(a => a.AssignedByUserId == user.UserId).ToList();
                db.ComplaintAssignments.RemoveRange(userAssignments);
            }
            
            // Remove the users
            db.Users.RemoveRange(usersToRemove);
            db.SaveChanges();
        }
    }
    catch { }
}

app.Run();


