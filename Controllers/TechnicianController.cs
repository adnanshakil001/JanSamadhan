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

        [Authorize(Roles = "Technician")]
        public async Task<IActionResult> Dashboard()
        {
            int technicianId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            
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
            return View(assignedComplaints);
        }

        [HttpGet]
        [Authorize(Roles = "Technician")]
        public async Task<IActionResult> Details(int id)
        {
            int technicianId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            
            var complaint = await _db.Complaints
                .Include(c => c.User)
                .Include(c => c.Department)
                .Include(c => c.TechnicianPhotos)
                .FirstOrDefaultAsync(c => c.ComplaintId == id && c.AssignedToUserId == technicianId);

            if (complaint == null)
            {
                return NotFound();
            }

            return View(complaint);
        }

        [HttpPost]
        [Authorize(Roles = "Technician")]
        public async Task<IActionResult> UploadPhotos(int complaintId, IFormFile workInProgressPhoto, IFormFile fixedPhoto)
        {
            int technicianId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            
            var complaint = await _db.Complaints
                .FirstOrDefaultAsync(c => c.ComplaintId == complaintId && c.AssignedToUserId == technicianId);

            if (complaint == null)
            {
                return Json(new { success = false, message = "Complaint not found or not assigned to you." });
            }

            if (workInProgressPhoto == null || fixedPhoto == null)
            {
                return Json(new { success = false, message = "Both photos are required." });
            }

            try
            {
                // Create uploads directory if it doesn't exist
                var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "technician");
                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                }

                // Generate unique filenames
                var workPhotoName = $"{Guid.NewGuid()}_{workInProgressPhoto.FileName}";
                var fixedPhotoName = $"{Guid.NewGuid()}_{fixedPhoto.FileName}";

                var workPhotoPath = Path.Combine(uploadsPath, workPhotoName);
                var fixedPhotoPath = Path.Combine(uploadsPath, fixedPhotoName);

                // Save photos
                using (var stream = new FileStream(workPhotoPath, FileMode.Create))
                {
                    await workInProgressPhoto.CopyToAsync(stream);
                }

                using (var stream = new FileStream(fixedPhotoPath, FileMode.Create))
                {
                    await fixedPhoto.CopyToAsync(stream);
                }

                // Save photo records to database
                var workPhoto = new TechnicianPhoto
                {
                    ComplaintId = complaintId,
                    PhotoType = "WorkInProgress",
                    PhotoPath = $"/uploads/technician/{workPhotoName}",
                    UploadedAt = DateTime.UtcNow
                };

                var fixedPhotoRecord = new TechnicianPhoto
                {
                    ComplaintId = complaintId,
                    PhotoType = "Fixed",
                    PhotoPath = $"/uploads/technician/{fixedPhotoName}",
                    UploadedAt = DateTime.UtcNow
                };

                _db.TechnicianPhotos.Add(workPhoto);
                _db.TechnicianPhotos.Add(fixedPhotoRecord);

                // Update complaint status to InProgress
            complaint.Status = ComplaintStatus.InProgress;
            complaint.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

                return Json(new { success = true, message = "Photos uploaded successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error uploading photos: " + ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Technician")]
        public async Task<IActionResult> MarkAsFixed(int complaintId)
        {
            int technicianId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            
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

            complaint.Status = ComplaintStatus.Fixed;
            complaint.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return Json(new { success = true, message = "Complaint marked as fixed successfully." });
        }

        [HttpGet]
        [Authorize(Roles = "Technician")]
        public async Task<IActionResult> AvailableWork()
        {
            int technicianId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            
            // Get technician's department
            var technician = await _db.Users
                .Include(u => u.Department)
                .FirstOrDefaultAsync(u => u.UserId == technicianId);

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
                .ThenBy(u => u.PendingTasks) // Secondary sort by pending tasks (ascending)
                .ToListAsync();

            return View(technicians);
        }
    }
}