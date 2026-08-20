using JanSamadhan.Data;
using JanSamadhan.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace JanSamadhan.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RoleController : Controller
    {
        private readonly ApplicationDbContext _db;

        public RoleController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(string? role, int? departmentId, string? q, string? sortBy = "Name", string? sortOrder = "asc")
        {
            var query = _db.Users
                .Include(u => u.Department)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(role))
            {
                query = query.Where(u => u.Role == role);
            }

            if (departmentId.HasValue)
            {
                query = query.Where(u => u.DepartmentId == departmentId.Value);
            }

            if (!string.IsNullOrEmpty(q))
            {
                query = query.Where(u => u.Name.Contains(q) || u.Email.Contains(q));
            }

            // Apply sorting
            query = sortBy.ToLower() switch
            {
                "name" => sortOrder == "asc" ? query.OrderBy(u => u.Name) : query.OrderByDescending(u => u.Name),
                "email" => sortOrder == "asc" ? query.OrderBy(u => u.Email) : query.OrderByDescending(u => u.Email),
                "role" => sortOrder == "asc" ? query.OrderBy(u => u.Role) : query.OrderByDescending(u => u.Role),
                "points" => sortOrder == "asc" ? query.OrderBy(u => u.Points) : query.OrderByDescending(u => u.Points),
                "department" => sortOrder == "asc" ? query.OrderBy(u => u.Department!.Name) : query.OrderByDescending(u => u.Department!.Name),
                _ => query.OrderBy(u => u.Name)
            };

            var users = await query.ToListAsync();

            // Prepare view data
            ViewBag.Role = role;
            ViewBag.DepartmentId = departmentId;
            ViewBag.Q = q;
            ViewBag.SortBy = sortBy;
            ViewBag.SortOrder = sortOrder;
            ViewBag.Departments = await _db.Departments.Where(d => d.IsActive).ToListAsync();
            ViewBag.Roles = new[] { "User", "Technician" };

            // Statistics
            ViewBag.TotalUsers = await _db.Users.CountAsync();
            ViewBag.TechnicianCount = await _db.Users.CountAsync(u => u.Role == "Technician");
            ViewBag.RegularUserCount = await _db.Users.CountAsync(u => u.Role == "User");

            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRole(int userId, string role, int? departmentId)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            // Validate role
            var validRoles = new[] { "User", "Technician" };
            if (!validRoles.Contains(role))
            {
                return Json(new { success = false, message = "Invalid role." });
            }

            user.Role = role;
            user.DepartmentId = departmentId;

            await _db.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> GetDepartments()
        {
            var departments = await _db.Departments
                .Where(d => d.IsActive)
                .Select(d => new { d.DepartmentId, d.Name })
                .ToListAsync();

            return Json(departments);
        }

        [HttpPost]
        public async Task<IActionResult> BulkUpdateRoles(int[] userIds, string role, int? departmentId)
        {
            try
            {
                var users = await _db.Users.Where(u => userIds.Contains(u.UserId)).ToListAsync();
                
                if (!users.Any())
                {
                    return Json(new { success = false, message = "No users found." });
                }

                // Validate role
                var validRoles = new[] { "User", "Technician" };
                if (!validRoles.Contains(role))
                {
                    return Json(new { success = false, message = "Invalid role." });
                }

                foreach (var user in users)
                {
                    user.Role = role;
                    user.DepartmentId = departmentId;
                }

                await _db.SaveChangesAsync();

                return Json(new { success = true, message = $"Updated {users.Count} users successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error updating users: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            try
            {
                var user = await _db.Users.FindAsync(userId);
                if (user == null)
                {
                    return Json(new { success = false, message = "User not found." });
                }

                // Check if user has technician role (prevent deleting technicians with active assignments)
                if (user.Role == "Technician")
                {
                    var activeAssignments = await _db.Complaints
                        .CountAsync(c => c.AssignedToUserId == userId && 
                                   (c.Status == ComplaintStatus.Pending || c.Status == ComplaintStatus.InProgress));
                    
                    if (activeAssignments > 0)
                    {
                        return Json(new { success = false, message = "Cannot delete technician with active assignments." });
                    }
                }

                // Remove related data
                var complaints = await _db.Complaints.Where(c => c.UserId == userId).ToListAsync();
                var assignedComplaints = await _db.Complaints.Where(c => c.AssignedToUserId == userId).ToListAsync();
                var ratings = await _db.ComplaintRatings.Where(r => r.UserId == userId).ToListAsync();
                var comments = await _db.Comments.Where(c => c.AuthorId == userId).ToListAsync();

                // Update assigned complaints to remove assignment
                foreach (var complaint in assignedComplaints)
                {
                    complaint.AssignedToUserId = null;
                }

                _db.Complaints.RemoveRange(complaints);
                _db.ComplaintRatings.RemoveRange(ratings);
                _db.Comments.RemoveRange(comments);
                _db.Users.Remove(user);

                await _db.SaveChangesAsync();

                return Json(new { success = true, message = "User deleted successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error deleting user: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetRoleStatistics()
        {
            var stats = new
            {
                TotalUsers = await _db.Users.CountAsync(),
                TechnicianCount = await _db.Users.CountAsync(u => u.Role == "Technician"),
                RegularUserCount = await _db.Users.CountAsync(u => u.Role == "User"),
                UsersByDepartment = await _db.Users
                    .Where(u => u.DepartmentId.HasValue)
                    .GroupBy(u => u.Department!.Name)
                    .Select(g => new { Department = g.Key, Count = g.Count() })
                    .ToListAsync()
            };

            return Json(stats);
        }

    }
}
