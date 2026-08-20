using JanSamadhan.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JanSamadhan.Controllers
{
    [Authorize(Roles = "User")]
    public class BadgeController : Controller
    {
        private readonly ApplicationDbContext _db;
        public BadgeController(ApplicationDbContext db) { _db = db; }

        public async Task<IActionResult> Index()
        {
            var badges = await _db.Badges.OrderBy(b => b.PointsRequired).ToListAsync();
            return View(badges);
        }
    }
}


