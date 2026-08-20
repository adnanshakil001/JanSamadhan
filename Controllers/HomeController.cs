using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using JanSamadhan.Data;

namespace JanSamadhan.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Scoreboard()
        {
            // Show only regular users (citizens), not technicians
            var topUsers = await _db.Users
                .Where(u => u.Role == "User")
                .OrderByDescending(u => u.Points)
                .Select(u => new { u.Name, u.BadgeLevel, u.Points })
                .ToListAsync();
            return View(topUsers);
        }

        [Authorize]
        public IActionResult Nearby()
        {
            return View();
        }
    }
}


