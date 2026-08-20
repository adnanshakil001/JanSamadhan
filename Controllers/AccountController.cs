using System.Security.Claims;
using JanSamadhan.Data;
using JanSamadhan.Models;
using JanSamadhan.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JanSamadhan.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IPasswordService _passwordService;

        public AccountController(ApplicationDbContext db, IPasswordService passwordService)
        {
            _db = db;
            _passwordService = passwordService;
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(string name, string email, string password)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError(string.Empty, "All fields are required.");
                return View();
            }

            if (await _db.Users.AnyAsync(u => u.Email == email))
            {
                ModelState.AddModelError(string.Empty, "Email already registered.");
                return View();
            }

            var user = new User
            {
                Name = name,
                Email = email,
                PasswordHash = _passwordService.HashPassword(password),
                Points = 0,
                BadgeLevel = "Bronze"
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null || !_passwordService.VerifyHashed(user.PasswordHash, password))
            {
                ModelState.AddModelError(string.Empty, "Invalid credentials.");
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, "User")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
            return RedirectToAction("Index", "Complaint");
        }

        [HttpGet]
        public IActionResult AdminLogin() => View();

        [HttpPost]
        public async Task<IActionResult> AdminLogin(string email, string password)
        {
            var admin = await _db.Admins.FirstOrDefaultAsync(a => a.Email == email);
            if (admin == null || !_passwordService.VerifyHashed(admin.PasswordHash, password))
            {
                ModelState.AddModelError(string.Empty, "Invalid credentials.");
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, admin.AdminId.ToString()),
                new Claim(ClaimTypes.Name, admin.Name),
                new Claim(ClaimTypes.Email, admin.Email),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
            return RedirectToAction("Index", "Admin");
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "User,Admin,Technician")]
        public async Task<IActionResult> Profile()
        {
            var role = User.FindFirstValue(ClaimTypes.Role);
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            
            if (role == "Admin")
            {
                var admin = await _db.Admins.FindAsync(userId);
                return View("AdminProfile", admin);
            }
            else if (role == "Technician")
            {
                var technician = await _db.Users
                    .Include(u => u.Department)
                    .FirstOrDefaultAsync(u => u.UserId == userId);
                return View("TechnicianProfile", technician);
            }
            else
            {
                var user = await _db.Users.FindAsync(userId);
                return View("UserProfile", user);
            }
        }

        public IActionResult AccessDenied() => View();

        [HttpGet]
        public IActionResult TechnicianLogin() => View();

        [HttpPost]
        public async Task<IActionResult> TechnicianLogin(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                TempData["ErrorMessage"] = "Email and password are required.";
                return View();
            }

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email && u.Role == "Technician");
            
            if (user == null || !_passwordService.VerifyHashed(user.PasswordHash, password))
            {
                TempData["ErrorMessage"] = "Invalid technician credentials.";
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, 
                new ClaimsPrincipal(claimsIdentity), authProperties);

            return RedirectToAction("Dashboard", "Technician");
        }
    }
}


