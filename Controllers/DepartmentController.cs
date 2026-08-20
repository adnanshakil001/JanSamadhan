using JanSamadhan.Data;
using JanSamadhan.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace JanSamadhan.Controllers
{
    [Authorize(Roles = "DepartmentManager")]
    public class DepartmentController : Controller
    {
        private readonly ApplicationDbContext _db;

        public DepartmentController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _db.Users
                .Include(u => u.Department)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user?.Department == null)
            {
                return Forbid("You are not assigned to a department.");
            }

            var complaints = await _db.Complaints
                .Include(c => c.User)
                .Include(c => c.AssignedToUser)
                .Where(c => c.DepartmentId == user.DepartmentId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            ViewBag.DepartmentName = user.Department.Name;
            ViewBag.TotalPending = complaints.Count(c => c.Status == ComplaintStatus.Pending);
            ViewBag.TotalInProgress = complaints.Count(c => c.Status == ComplaintStatus.InProgress);
            ViewBag.TotalFixed = complaints.Count(c => c.Status == ComplaintStatus.Fixed);

            return View(complaints);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, ComplaintStatus status)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _db.Users
                .Include(u => u.Department)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user?.Department == null)
            {
                return Forbid("You are not assigned to a department.");
            }

            var complaint = await _db.Complaints.FindAsync(id);
            if (complaint == null || complaint.DepartmentId != user.DepartmentId)
            {
                return NotFound();
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
                AuthorName = User.FindFirstValue(ClaimTypes.Name) ?? "Department Manager",
                AuthorRole = "DepartmentManager",
                Content = $"Status changed from {oldStatus} to {status} by Department Manager",
                VisibleToUser = true,
                CreatedAt = DateTime.UtcNow
            };

            _db.Comments.Add(statusChangeComment);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Assign(int id, int? assignedToUserId, string? note)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _db.Users
                .Include(u => u.Department)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user?.Department == null)
            {
                return Forbid("You are not assigned to a department.");
            }

            var complaint = await _db.Complaints.FindAsync(id);
            if (complaint == null || complaint.DepartmentId != user.DepartmentId)
            {
                return NotFound();
            }

            // Deactivate previous assignments
            var previousAssignments = await _db.ComplaintAssignments
                .Where(a => a.ComplaintId == id && a.IsActive)
                .ToListAsync();

            foreach (var assignment in previousAssignments)
            {
                assignment.IsActive = false;
            }

            // Create new assignment
            var newAssignment = new ComplaintAssignment
            {
                ComplaintId = id,
                DepartmentId = user.DepartmentId,
                AssignedToUserId = assignedToUserId,
                Note = note,
                AssignedByUserId = userId,
                AssignedAt = DateTime.UtcNow,
                IsActive = true
            };

            _db.ComplaintAssignments.Add(newAssignment);

            // Update complaint
            complaint.AssignedToUserId = assignedToUserId;
            complaint.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            // Add assignment comment
            var assignmentComment = new Comment
            {
                ComplaintId = id,
                AuthorId = userId,
                AuthorName = User.FindFirstValue(ClaimTypes.Name) ?? "Department Manager",
                AuthorRole = "DepartmentManager",
                Content = $"Complaint assigned to technician by Department Manager",
                VisibleToUser = true,
                CreatedAt = DateTime.UtcNow
            };

            _db.Comments.Add(assignmentComment);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
