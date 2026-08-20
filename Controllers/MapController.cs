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
            var query = _db.Complaints.Include(c => c.User).Include(c => c.Department).AsQueryable();

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
                    userName = c.User!.Name,
                    createdAt = c.CreatedAt,
                    badge = c.User.BadgeLevel,
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
        public async Task<IActionResult> Nearby(double lat, double lng, int radiusMeters = 200)
        {
            // Debug: Log the search parameters
            Console.WriteLine($"Searching for complaints near lat: {lat}, lng: {lng}, radius: {radiusMeters}m");
            
            // Debug: Check total complaints in database
            var totalComplaints = await _db.Complaints.CountAsync();
            Console.WriteLine($"Total complaints in database: {totalComplaints}");
            
            // Simple bounding box search (not precise circle, but good enough for most cases)
            var latRange = radiusMeters / 111000.0; // Rough conversion: 1 degree ≈ 111km
            var lngRange = radiusMeters / (111000.0 * Math.Cos(lat * Math.PI / 180.0));

            // Debug: Log the search range
            Console.WriteLine($"Search range: lat ±{latRange}, lng ±{lngRange}");

            var nearbyComplaints = await _db.Complaints
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
                    UserName = c.User!.Name,
                    Distance = Math.Sqrt(Math.Pow(c.Latitude - lat, 2) + Math.Pow(c.Longitude - lng, 2)) * 111000
                })
                .OrderBy(c => c.Distance)
                .Take(10)
                .ToListAsync();

            // Debug: Log the complaints being returned
            Console.WriteLine($"Returning {nearbyComplaints.Count} nearby complaints");
            foreach (var complaint in nearbyComplaints)
            {
                Console.WriteLine($"Complaint: {complaint.ProblemType}, Status: {complaint.Status}, User: {complaint.UserName}");
            }
            
            // If no nearby complaints found, return all complaints for testing
            if (nearbyComplaints.Count == 0)
            {
                Console.WriteLine("No nearby complaints found, returning all complaints for testing");
                var allComplaints = await _db.Complaints
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
                        UserName = c.User!.Name,
                        Distance = Math.Sqrt(Math.Pow(c.Latitude - lat, 2) + Math.Pow(c.Longitude - lng, 2)) * 111000
                    })
                    .OrderBy(c => c.Distance)
                    .Take(5)
                    .ToListAsync();
                
                Console.WriteLine($"Returning {allComplaints.Count} all complaints for testing");
                
                // If still no complaints, return a test complaint
                if (allComplaints.Count == 0)
                {
                    Console.WriteLine("No complaints in database, returning test complaint");
                    var testComplaint = new[]
                    {
                        new
                        {
                            ComplaintId = 1,
                            ProblemType = "Test Pothole",
                            Status = "Pending",
                            CreatedAt = DateTime.UtcNow,
                            Address = "Test Address",
                            PhotoPath = (string)null,
                            Latitude = lat,
                            Longitude = lng,
                            UserName = "Test User",
                            Distance = 0.0
                        }
                    };
                    return Json(testComplaint);
                }
                
                return Json(allComplaints);
            }
            
            return Json(nearbyComplaints);
        }
    }
}
