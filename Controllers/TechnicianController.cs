using JanSamadhan.Data;
using JanSamadhan.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace JanSamadhan.Controllers
{
    [Authorize(Roles = "Technician")]
    public class TechnicianController : Controller
    {
        private readonly ApplicationDbContext _db;

        public TechnicianController(ApplicationDbContext db)
        {
            _db = db;
        }

        private async Task<User?> GetCurrentTechnicianAsync()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int.TryParse(idClaim, out int techId);
            var email = User.FindFirstValue(ClaimTypes.Email);
            return await _db.Users
                .Include(u => u.Department)
                .FirstOrDefaultAsync(u => (u.UserId == techId || (!string.IsNullOrEmpty(email) && u.Email == email)) && u.Role == "Technician");
        }

        [Authorize(Roles = "Technician")]
        public async Task<IActionResult> Dashboard()
        {
            var technician = await GetCurrentTechnicianAsync();
            int technicianId = technician?.UserId ?? 0;
            
            var assignedComplaints = await _db.Complaints
                .Include(c => c.User)
                .Include(c => c.Department)
                .Where(c => c.AssignedToUserId == technicianId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            var stats = new
            {
                TotalAssigned = assignedComplaints.Count,
                Pending = assignedComplaints.Count(c => c.Status == ComplaintStatus.Pending),
                InProgress = assignedComplaints.Count(c => c.Status == ComplaintStatus.InProgress),
                Fixed = assignedComplaints.Count(c => c.Status == ComplaintStatus.Fixed)
            };

            ViewBag.Stats = stats;
            ViewBag.TechnicianName = technician?.Name ?? (User.FindFirstValue(ClaimTypes.Name) ?? "Technician");
            ViewBag.DepartmentName = technician?.Department?.Name ?? "Municipal Department";
            ViewBag.Points = technician?.Points ?? 0;
            ViewBag.BadgeLevel = technician?.BadgeLevel ?? "Bronze";

            return View(assignedComplaints);
        }

        [HttpGet]
        [Authorize(Roles = "Technician")]
        public async Task<IActionResult> Details(int id)
        {
            var technician = await GetCurrentTechnicianAsync();
            int technicianId = technician?.UserId ?? 0;
            
            var complaint = await _db.Complaints
                .Include(c => c.User)
                .Include(c => c.Department)
                .Include(c => c.TechnicianPhotos)
                .FirstOrDefaultAsync(c => c.ComplaintId == id && 
                    (c.AssignedToUserId == technicianId || (technician != null && c.DepartmentId == technician.DepartmentId && c.AssignedToUserId == null)));

            if (complaint == null)
            {
                return NotFound();
            }

            ViewBag.IsAssignedToMe = (complaint.AssignedToUserId == technicianId);
            ViewBag.TechnicianName = technician?.Name ?? "Technician";
            ViewBag.DepartmentName = technician?.Department?.Name ?? "Department";
            ViewBag.Points = technician?.Points ?? 0;
            ViewBag.BadgeLevel = technician?.BadgeLevel ?? "Bronze";

            return View(complaint);
        }

        [HttpPost]
        [Authorize(Roles = "Technician")]
        public async Task<IActionResult> Claim(int id)
        {
            var technician = await GetCurrentTechnicianAsync();
            if (technician == null)
            {
                return Json(new { success = false, message = "Technician profile not found." });
            }
            int technicianId = technician.UserId;

            var complaint = await _db.Complaints
                .FirstOrDefaultAsync(c => c.ComplaintId == id && 
                    (c.AssignedToUserId == null && c.DepartmentId == technician.DepartmentId));

            if (complaint == null)
            {
                return Json(new { success = false, message = "Complaint not found or already assigned." });
            }

            complaint.AssignedToUserId = technicianId;
            complaint.Status = ComplaintStatus.InProgress;
            complaint.UpdatedAt = DateTime.UtcNow;

            var assignment = new ComplaintAssignment
            {
                ComplaintId = complaint.ComplaintId,
                DepartmentId = complaint.DepartmentId ?? technician.DepartmentId ?? 0,
                AssignedToUserId = technicianId,
                AssignedByUserId = technicianId,
                AssignedAt = DateTime.UtcNow,
                IsActive = true,
                Note = $"Claimed by technician {technician.Name}"
            };
            _db.ComplaintAssignments.Add(assignment);

            await _db.SaveChangesAsync();
            return Json(new { success = true, message = "Complaint claimed successfully." });
        }

        [HttpPost]
        [Authorize(Roles = "Technician")]
        public async Task<IActionResult> UploadPhotos(int complaintId, IFormFile? workInProgressPhoto, IFormFile? fixedPhoto)
        {
            var technician = await GetCurrentTechnicianAsync();
            if (technician == null)
            {
                return Json(new { success = false, message = "Technician profile not found." });
            }
            int technicianId = technician.UserId;
            
            var complaint = await _db.Complaints
                .Include(c => c.TechnicianPhotos)
                .FirstOrDefaultAsync(c => c.ComplaintId == complaintId && 
                    (c.AssignedToUserId == technicianId || (c.DepartmentId == technician.DepartmentId && c.AssignedToUserId == null)));

            if (complaint == null)
            {
                return Json(new { success = false, message = "Complaint not found or not in your department." });
            }

            if (workInProgressPhoto == null && fixedPhoto == null)
            {
                return Json(new { success = false, message = "Please select at least one photo to upload." });
            }

            if (complaint.AssignedToUserId == null)
            {
                complaint.AssignedToUserId = technicianId;
            }

            try
            {
                var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "technician");
                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                }

                string statusMessage = "";

                // Helper for photo validation
                bool IsValidImage(IFormFile file)
                {
                    var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                    var allowedExts = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
                    return allowedExts.Contains(ext) && file.Length <= 15 * 1024 * 1024; // 15MB limit
                }

                // 1. Handle Work in Progress Photo
                if (workInProgressPhoto != null && workInProgressPhoto.Length > 0)
                {
                    if (!IsValidImage(workInProgressPhoto))
                    {
                        return Json(new { success = false, message = "Work-in-progress photo must be a valid image (JPG, PNG, WebP) under 15MB." });
                    }

                    var workPhotoName = $"{Guid.NewGuid()}_{Path.GetFileName(workInProgressPhoto.FileName)}";
                    var workPhotoPath = Path.Combine(uploadsPath, workPhotoName);

                    using (var stream = new FileStream(workPhotoPath, FileMode.Create))
                    {
                        await workInProgressPhoto.CopyToAsync(stream);
                    }

                    _db.TechnicianPhotos.Add(new TechnicianPhoto
                    {
                        ComplaintId = complaintId,
                        PhotoType = "WorkInProgress",
                        PhotoPath = $"/uploads/technician/{workPhotoName}",
                        UploadedAt = DateTime.UtcNow
                    });

                    if (complaint.Status == ComplaintStatus.Pending)
                    {
                        complaint.Status = ComplaintStatus.InProgress;
                        statusMessage = "Work marked as In Progress with uploaded photo.";
                    }
                }

                // 2. Handle Fixed / Completion Photo
                if (fixedPhoto != null && fixedPhoto.Length > 0)
                {
                    if (!IsValidImage(fixedPhoto))
                    {
                        return Json(new { success = false, message = "Fixed photo must be a valid image (JPG, PNG, WebP) under 15MB." });
                    }

                    var fixedPhotoName = $"{Guid.NewGuid()}_{Path.GetFileName(fixedPhoto.FileName)}";
                    var fixedPhotoPath = Path.Combine(uploadsPath, fixedPhotoName);

                    using (var stream = new FileStream(fixedPhotoPath, FileMode.Create))
                    {
                        await fixedPhoto.CopyToAsync(stream);
                    }

                    _db.TechnicianPhotos.Add(new TechnicianPhoto
                    {
                        ComplaintId = complaintId,
                        PhotoType = "Fixed",
                        PhotoPath = $"/uploads/technician/{fixedPhotoName}",
                        UploadedAt = DateTime.UtcNow
                    });

                    bool wasAlreadyFixed = (complaint.Status == ComplaintStatus.Fixed);
                    complaint.Status = ComplaintStatus.Fixed;
                    statusMessage = "Work marked as Fixed with completion photo! You earned 10 points.";

                    // Award points and upgrade badge if not previously fixed
                    if (!wasAlreadyFixed)
                    {
                        technician.Points += 10;
                        var badges = await _db.Badges.OrderByDescending(b => b.PointsRequired).ToListAsync();
                        var earnedBadge = badges.FirstOrDefault(b => technician.Points >= b.PointsRequired);
                        if (earnedBadge != null)
                        {
                            technician.BadgeLevel = earnedBadge.LevelName;
                        }
                    }
                }

                complaint.UpdatedAt = DateTime.UtcNow;

                // Log comment in conversation thread
                _db.Comments.Add(new Comment
                {
                    ComplaintId = complaintId,
                    AuthorId = technicianId,
                    AuthorName = technician.Name,
                    AuthorRole = "Technician",
                    Content = $"Technician updated status to {complaint.Status} and uploaded photo proof.",
                    VisibleToUser = true,
                    CreatedAt = DateTime.UtcNow
                });

                await _db.SaveChangesAsync();

                return Json(new { 
                    success = true, 
                    message = string.IsNullOrEmpty(statusMessage) ? "Photo uploaded successfully." : statusMessage,
                    status = complaint.Status.ToString()
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error uploading photo: " + ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Technician")]
        public async Task<IActionResult> MarkAsFixed(int complaintId)
        {
            var technician = await GetCurrentTechnicianAsync();
            if (technician == null)
            {
                return Json(new { success = false, message = "Technician profile not found." });
            }
            int technicianId = technician.UserId;
            
            var complaint = await _db.Complaints
                .FirstOrDefaultAsync(c => c.ComplaintId == complaintId && c.AssignedToUserId == technicianId);

            if (complaint == null)
            {
                return Json(new { success = false, message = "Complaint not found or not assigned to you." });
            }

            // Check if both photos are uploaded
            var photos = await _db.TechnicianPhotos
                .Where(p => p.ComplaintId == complaintId)
                .ToListAsync();

            if (photos.Count < 2)
            {
                return Json(new { success = false, message = "Please upload both work in progress and fixed photos before marking as complete." });
            }

            bool wasAlreadyFixed = (complaint.Status == ComplaintStatus.Fixed);
            complaint.Status = ComplaintStatus.Fixed;
            complaint.UpdatedAt = DateTime.UtcNow;

            // Award points and upgrade badge for technician if not already awarded
            if (!wasAlreadyFixed)
            {
                technician.Points += 10;
                var badges = await _db.Badges.OrderByDescending(b => b.PointsRequired).ToListAsync();
                var earnedBadge = badges.FirstOrDefault(b => technician.Points >= b.PointsRequired);
                if (earnedBadge != null)
                {
                    technician.BadgeLevel = earnedBadge.LevelName;
                }
            }

            await _db.SaveChangesAsync();

            return Json(new { success = true, message = "Complaint marked as fixed successfully!" });
        }

        [HttpGet]
        [Authorize(Roles = "Technician")]
        public async Task<IActionResult> AvailableWork()
        {
            var technician = await GetCurrentTechnicianAsync();
            int technicianId = technician?.UserId ?? 0;
            
            ViewBag.TechnicianName = technician?.Name ?? "Technician";
            ViewBag.DepartmentName = technician?.Department?.Name ?? "Department";
            ViewBag.Points = technician?.Points ?? 0;
            ViewBag.BadgeLevel = technician?.BadgeLevel ?? "Bronze";

            if (technician?.DepartmentId == null)
            {
                ViewBag.Message = "You are not assigned to any department. Please contact admin.";
                return View(new List<Complaint>());
            }

            // Get available complaints for technician's department that are not assigned
            var availableComplaints = await _db.Complaints
                .Include(c => c.User)
                .Include(c => c.Department)
                .Where(c => c.DepartmentId == technician.DepartmentId && 
                           c.AssignedToUserId == null && 
                           c.Status == ComplaintStatus.Pending)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            ViewBag.TechnicianDepartment = technician.Department?.Name;
            return View(availableComplaints);
        }

        [HttpGet]
        [Authorize(Roles = "Technician")]
        public async Task<IActionResult> Scoreboard()
        {
            var technician = await GetCurrentTechnicianAsync();
            ViewBag.TechnicianName = technician?.Name ?? "Technician";
            ViewBag.DepartmentName = technician?.Department?.Name ?? "Department";
            ViewBag.Points = technician?.Points ?? 0;
            ViewBag.BadgeLevel = technician?.BadgeLevel ?? "Bronze";

            // Show technician scoreboard ranked by completed tasks
            var technicians = await _db.Users
                .Where(u => u.Role == "Technician")
                .Include(u => u.Department)
                .Select(u => new { 
                    u.Name, 
                    u.Email, 
                    DepartmentName = u.Department != null ? u.Department.Name : "Unassigned",
                    CompletedTasks = u.AssignedComplaints.Count(c => c.Status == ComplaintStatus.Fixed),
                    PendingTasks = u.AssignedComplaints.Count(c => c.Status == ComplaintStatus.Pending || c.Status == ComplaintStatus.InProgress)
                })
                .OrderByDescending(u => u.CompletedTasks)
                .ThenBy(u => u.PendingTasks)
                .ToListAsync();

            return View(technicians);
        }
    }
}