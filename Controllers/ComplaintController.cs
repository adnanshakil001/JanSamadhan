using System.Security.Claims;
using JanSamadhan.Data;
using JanSamadhan.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JanSamadhan.Controllers
{
    [Authorize(Roles = "User")]
    public class ComplaintController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;

        public ComplaintController(ApplicationDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var userRole = User.FindFirstValue(ClaimTypes.Role) ?? "User";
            
            IQueryable<Complaint> query;
            
            if (userRole == "Technician")
            {
                // Technicians see their assigned complaints
                query = _db.Complaints.Where(c => c.AssignedToUserId == userId);
            }
            else
            {
                // Regular users see their own complaints
                query = _db.Complaints.Where(c => c.UserId == userId);
            }
            
            var complaints = await query
                .Include(c => c.User)
                .Include(c => c.Department)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
                
            ViewBag.UserRole = userRole;
            return View(complaints);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(string problemType, string? description, double latitude, double longitude, string? address, IFormFile? photo)
        {
            if (string.IsNullOrWhiteSpace(problemType))
            {
                ModelState.AddModelError(string.Empty, "Problem type is required.");
                return View();
            }

            string? photoPath = null;
            if (photo != null && photo.Length > 0)
            {
                string uploads = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploads);
                string fileName = $"{Guid.NewGuid()}{Path.GetExtension(photo.FileName)}";
                string filePath = Path.Combine(uploads, fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    await photo.CopyToAsync(stream);
                }
                photoPath = $"/uploads/{fileName}";
            }

            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var complaint = new Complaint
            {
                UserId = userId,
                ProblemType = problemType,
                Description = description,
                PhotoPath = photoPath,
                Latitude = latitude,
                Longitude = longitude,
                Address = address,
                Status = ComplaintStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _db.Complaints.Add(complaint);
            await _db.SaveChangesAsync();

            // Auto-assign based on problem type
            var mapping = await _db.ProblemTypeMappings
                .Include(m => m.Department)
                .FirstOrDefaultAsync(m => m.ProblemType == problemType && m.IsActive);
            
            if (mapping != null)
            {
                complaint.DepartmentId = mapping.DepartmentId;
                complaint.UpdatedAt = DateTime.UtcNow;
                
                // Create assignment record
                var assignment = new ComplaintAssignment
                {
                    ComplaintId = complaint.ComplaintId,
                    DepartmentId = mapping.DepartmentId,
                    AssignedByUserId = userId, // System assignment
                    AssignedAt = DateTime.UtcNow,
                    IsActive = true,
                    Note = "Auto-assigned based on problem type"
                };
                
                _db.ComplaintAssignments.Add(assignment);
                
                // Add assignment comment
                var assignmentComment = new Comment
                {
                    ComplaintId = complaint.ComplaintId,
                    AuthorId = userId,
                    AuthorName = "System",
                    AuthorRole = "System",
                    Content = $"Complaint automatically assigned to {mapping.Department?.Name ?? "Unknown"} department",
                    VisibleToUser = true,
                    CreatedAt = DateTime.UtcNow
                };
                
                _db.Comments.Add(assignmentComment);
            }

            await _db.SaveChangesAsync();

            // award points
            var user = await _db.Users.FindAsync(userId);
            if (user != null)
            {
                user.Points += 5; // basic gamification point
                user.BadgeLevel = await ComputeBadgeLevel(user.Points);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var complaint = await _db.Complaints
                .Include(c => c.User)
                .Include(c => c.Department)
                .Include(c => c.AssignedToUser)
                .Include(c => c.Rating)
                .FirstOrDefaultAsync(c => c.ComplaintId == id);

            if (complaint == null)
            {
                return NotFound();
            }

            // Check if user has permission to view this complaint
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var userRole = User.FindFirstValue(ClaimTypes.Role) ?? "User";
            
            if (userRole == "User" && complaint.UserId != userId)
            {
                return Forbid();
            }

            return View(complaint);
        }

        [HttpPost]
        public async Task<IActionResult> Rate(int complaintId, int score, string? comment)
        {
            if (score < 1 || score > 5)
            {
                return Json(new { success = false, message = "Rating must be between 1 and 5." });
            }

            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var complaint = await _db.Complaints.FindAsync(complaintId);
            
            if (complaint == null)
            {
                return Json(new { success = false, message = "Complaint not found." });
            }

            if (complaint.UserId != userId)
            {
                return Json(new { success = false, message = "You can only rate your own complaints." });
            }

            if (complaint.Status != ComplaintStatus.Fixed)
            {
                return Json(new { success = false, message = "You can only rate fixed complaints." });
            }

            // Check if user already rated this complaint
            var existingRating = await _db.ComplaintRatings
                .FirstOrDefaultAsync(r => r.ComplaintId == complaintId && r.UserId == userId);

            if (existingRating != null)
            {
                // Check if it's within 24 hours for editing
                if (existingRating.CreatedAt.AddHours(24) < DateTime.UtcNow)
                {
                    return Json(new { success = false, message = "You can only edit your rating within 24 hours." });
                }

                existingRating.Score = score;
                existingRating.Comment = comment;
                existingRating.LastModifiedAt = DateTime.UtcNow;
            }
            else
            {
                var rating = new ComplaintRating
                {
                    ComplaintId = complaintId,
                    UserId = userId,
                    Score = score,
                    Comment = comment,
                    CreatedAt = DateTime.UtcNow
                };

                _db.ComplaintRatings.Add(rating);
                complaint.RatingId = rating.RatingId;
            }

            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, ComplaintStatus status)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var userRole = User.FindFirstValue(ClaimTypes.Role) ?? "User";
            
            var complaint = await _db.Complaints.FindAsync(id);
            if (complaint == null)
            {
                return Json(new { success = false, message = "Complaint not found." });
            }

            // Check permissions
            if (userRole == "Technician" && complaint.AssignedToUserId != userId)
            {
                return Json(new { success = false, message = "You can only update complaints assigned to you." });
            }

            var oldStatus = complaint.Status;
            complaint.Status = status;
            complaint.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            // Add status change comment
            var statusChangeComment = new Comment
            {
                ComplaintId = id,
                AuthorId = userId,
                AuthorName = User.FindFirstValue(ClaimTypes.Name) ?? "User",
                AuthorRole = userRole,
                Content = $"Status changed from {oldStatus} to {status}",
                VisibleToUser = true,
                CreatedAt = DateTime.UtcNow
            };

            _db.Comments.Add(statusChangeComment);
            await _db.SaveChangesAsync();

            return Json(new { success = true });
        }

        private async Task<string> ComputeBadgeLevel(int points)
        {
            var badges = await _db.Badges.OrderBy(b => b.PointsRequired).ToListAsync();
            string level = "Bronze";
            foreach (var b in badges)
            {
                if (points >= b.PointsRequired) level = b.LevelName;
            }
            return level;
        }
    }
}


