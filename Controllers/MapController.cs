using JanSamadhan.Data;
using JanSamadhan.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace JanSamadhan.Controllers
{
    [Authorize]
    public class MapController : Controller
    {
        private readonly ApplicationDbContext _db;

        public MapController(ApplicationDbContext db)
        {
            _db = db;
        }

        [Authorize(Roles = "Admin,DepartmentManager")]
        public IActionResult AdminMap()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> MapData(string? status, DateTime? from, DateTime? to, int? departmentId)
        {
            var query = _db.Complaints.AsNoTracking().Include(c => c.User).Include(c => c.Department).AsQueryable();

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<ComplaintStatus>(status, true, out var parsedStatus))
            {
                query = query.Where(c => c.Status == parsedStatus);
            }

            if (from.HasValue)
            {
                query = query.Where(c => c.CreatedAt >= from.Value);
            }

            if (to.HasValue)
            {
                query = query.Where(c => c.CreatedAt <= to.Value);
            }

            if (departmentId.HasValue)
            {
                query = query.Where(c => c.DepartmentId == departmentId.Value);
            }

            var complaints = await query
                .Select(c => new
                {
                    id = c.ComplaintId,
                    status = c.Status.ToString(),
                    type = c.ProblemType,
                    userName = c.User != null ? c.User.Name : "Anonymous",
                    createdAt = c.CreatedAt,
                    badge = c.User != null ? c.User.BadgeLevel : "Bronze",
                    photoUrl = c.PhotoPath,
                    coordinates = new[] { c.Longitude, c.Latitude }
                })
                .ToListAsync();

            var geoJson = new
            {
                type = "FeatureCollection",
                features = complaints.Select(c => new
                {
                    type = "Feature",
                    geometry = new
                    {
                        type = "Point",
                        coordinates = c.coordinates
                    },
                    properties = new
                    {
                        c.id,
                        c.status,
                        c.type,
                        c.userName,
                        createdAt = c.createdAt.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        c.badge,
                        c.photoUrl
                    }
                })
            };

            return Json(geoJson);
        }

        [HttpGet]
        public async Task<IActionResult> Nearby(double lat, double lng, int radiusMeters = 500)
        {
            // Simple bounding box search (1 degree ≈ 111km)
            var latRange = radiusMeters / 111000.0;
            var lngRange = radiusMeters / (111000.0 * Math.Cos(lat * Math.PI / 180.0));

            var nearbyComplaints = await _db.Complaints
                .AsNoTracking()
                .Where(c => c.Latitude >= lat - latRange && c.Latitude <= lat + latRange &&
                           c.Longitude >= lng - lngRange && c.Longitude <= lng + lngRange)
                .Include(c => c.User)
                .Select(c => new
                {
                    ComplaintId = c.ComplaintId,
                    ProblemType = c.ProblemType,
                    Status = c.Status.ToString(),
                    CreatedAt = c.CreatedAt,
                    Address = c.Address,
                    PhotoPath = c.PhotoPath,
                    Latitude = c.Latitude,
                    Longitude = c.Longitude,
                    UserName = c.User != null ? c.User.Name : "Anonymous",
                    Distance = Math.Sqrt(Math.Pow(c.Latitude - lat, 2) + Math.Pow(c.Longitude - lng, 2)) * 111000
                })
                .OrderBy(c => c.Distance)
                .Take(20)
                .ToListAsync();

            return Json(nearbyComplaints);
        }
    }
}
