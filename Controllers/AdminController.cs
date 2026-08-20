using JanSamadhan.Data;
using JanSamadhan.Models;
using JanSamadhan.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;

namespace JanSamadhan.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IPasswordService _passwordService;

        public AdminController(ApplicationDbContext db, IPasswordService passwordService)
        {
            _db = db;
            _passwordService = passwordService;
        }

        public async Task<IActionResult> Index(string? status, int? departmentId, int? assignedToUserId, string? q, string? sortBy = "CreatedAt", string? sortOrder = "desc", int page = 1, int pageSize = 20)
        {
            ViewBag.TotalPending = await _db.Complaints.CountAsync(c => c.Status == ComplaintStatus.Pending);
            ViewBag.TotalInProgress = await _db.Complaints.CountAsync(c => c.Status == ComplaintStatus.InProgress);
            ViewBag.TotalFixed = await _db.Complaints.CountAsync(c => c.Status == ComplaintStatus.Fixed);

            var query = _db.Complaints
                .Include(c => c.User)
                .Include(c => c.Department)
                .Include(c => c.AssignedToUser)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<ComplaintStatus>(status, true, out var parsedStatus))
            {
                query = query.Where(c => c.Status == parsedStatus);
            }

            if (departmentId.HasValue)
            {
                query = query.Where(c => c.DepartmentId == departmentId.Value);
            }

            if (assignedToUserId.HasValue)
            {
                query = query.Where(c => c.AssignedToUserId == assignedToUserId.Value);
            }

            if (!string.IsNullOrEmpty(q))
            {
                query = query.Where(c => c.ProblemType.Contains(q) || 
                                       (c.Description != null && c.Description.Contains(q)) ||
                                       (c.Address != null && c.Address.Contains(q)) ||
                                       c.User!.Name.Contains(q));
            }

            // Apply sorting
            query = sortBy.ToLower() switch
            {
                "createdat" => sortOrder == "asc" ? query.OrderBy(c => c.CreatedAt) : query.OrderByDescending(c => c.CreatedAt),
                "status" => sortOrder == "asc" ? query.OrderBy(c => c.Status) : query.OrderByDescending(c => c.Status),
                "problemtype" => sortOrder == "asc" ? query.OrderBy(c => c.ProblemType) : query.OrderByDescending(c => c.ProblemType),
                "username" => sortOrder == "asc" ? query.OrderBy(c => c.User!.Name) : query.OrderByDescending(c => c.User!.Name),
                "department" => sortOrder == "asc" ? query.OrderBy(c => c.Department!.Name) : query.OrderByDescending(c => c.Department!.Name),
                _ => query.OrderByDescending(c => c.CreatedAt)
            };

            // Get total count for pagination
            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            // Apply pagination
            var complaints = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Prepare view data
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.Status = status;
            ViewBag.DepartmentId = departmentId;
            ViewBag.AssignedToUserId = assignedToUserId;
            ViewBag.Q = q;
            ViewBag.SortBy = sortBy;
            ViewBag.SortOrder = sortOrder;

            // Get filter options
            ViewBag.Departments = await _db.Departments.Where(d => d.IsActive).ToListAsync();
            ViewBag.Users = await _db.Users.Where(u => u.Role != "User").ToListAsync();

            return View(complaints);
        }

        [HttpGet]
        public async Task<IActionResult> Scoreboard()
        {
            // Show only regular users (citizens), not technicians
            var topUsers = await _db.Users
                .Where(u => u.Role == "User")
                .OrderByDescending(u => u.Points)
                .Select(u => new { u.Name, u.Email, u.Points, u.BadgeLevel })
                .ToListAsync();
            return View(topUsers);
        }

        [HttpGet]
        public async Task<IActionResult> TechnicianScoreboard()
        {
            // Show only technicians (workers) ranked by completed tasks
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

        [HttpGet]
        public async Task<IActionResult> Filter(string? status)
        {
            IQueryable<Complaint> query = _db.Complaints.Include(c => c.User);
            if (!string.IsNullOrWhiteSpace(status))
            {
                if (Enum.TryParse<ComplaintStatus>(status, true, out var parsed))
                {
                    query = query.Where(c => c.Status == parsed);
                }
            }

            var complaints = await query.OrderByDescending(c => c.CreatedAt).ToListAsync();
            return View("Index", complaints);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var complaint = await _db.Complaints
                .Include(c => c.User)
                .Include(c => c.Department)
                .Include(c => c.AssignedToUser)
                .Include(c => c.Rating)
                .Include(c => c.TechnicianPhotos)
                .FirstOrDefaultAsync(c => c.ComplaintId == id);

            if (complaint == null)
            {
                return NotFound();
            }

            return View(complaint);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, ComplaintStatus status)
        {
            var complaint = await _db.Complaints.FindAsync(id);
            if (complaint == null) return NotFound();
            
            var oldStatus = complaint.Status;
            
            // Validate status transition - prevent illogical changes
            if (!IsValidStatusTransition(oldStatus, status))
            {
                TempData["ErrorMessage"] = $"Cannot change status from {oldStatus} to {status}. Invalid status transition.";
                return RedirectToAction(nameof(Index));
            }
            
            complaint.Status = status;
            complaint.UpdatedAt = DateTime.UtcNow;
            
            await _db.SaveChangesAsync();
            
            // Add status change comment
            var statusChangeComment = new Comment
            {
                ComplaintId = id,
                AuthorId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),
                AuthorName = User.FindFirstValue(ClaimTypes.Name) ?? "Admin",
                AuthorRole = "Admin",
                Content = $"Status changed from {oldStatus} to {status}",
                VisibleToUser = true,
                CreatedAt = DateTime.UtcNow
            };
            
            _db.Comments.Add(statusChangeComment);
            await _db.SaveChangesAsync();
            
            return RedirectToAction(nameof(Index));
        }

        private bool IsValidStatusTransition(ComplaintStatus fromStatus, ComplaintStatus toStatus)
        {
            // Define valid status transitions
            return fromStatus switch
            {
                ComplaintStatus.Pending => toStatus == ComplaintStatus.InProgress || toStatus == ComplaintStatus.Fixed,
                ComplaintStatus.InProgress => toStatus == ComplaintStatus.Fixed,
                ComplaintStatus.Fixed => false, // Fixed complaints cannot be changed to any other status
                _ => false
            };
        }

        [HttpPost]
        public async Task<IActionResult> Assign(int id, int? departmentId, int? assignedToUserId, string? note)
        {
            var complaint = await _db.Complaints.FindAsync(id);
            if (complaint == null) return NotFound();

            // Deactivate previous assignments
            var previousAssignments = await _db.ComplaintAssignments
                .Where(a => a.ComplaintId == id && a.IsActive)
                .ToListAsync();
            
            foreach (var assignment in previousAssignments)
            {
                assignment.IsActive = false;
            }

            // Check if technician has pending work (only if assigning to a technician)
            if (assignedToUserId.HasValue)
            {
                var pendingWork = await _db.Complaints
                    .CountAsync(c => c.AssignedToUserId == assignedToUserId.Value && 
                                   (c.Status == ComplaintStatus.Pending || c.Status == ComplaintStatus.InProgress));

                if (pendingWork > 0)
                {
                    return Json(new { success = false, message = "Technician has pending work. Complete current tasks before assigning new ones." });
                }
            }

            // Create new assignment
            var newAssignment = new ComplaintAssignment
            {
                ComplaintId = id,
                DepartmentId = departmentId,
                AssignedToUserId = assignedToUserId,
                Note = note,
                AssignedByUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),
                AssignedAt = DateTime.UtcNow,
                IsActive = true
            };

            _db.ComplaintAssignments.Add(newAssignment);

            // Update complaint
            complaint.DepartmentId = departmentId;
            complaint.AssignedToUserId = assignedToUserId;
            complaint.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            // Add assignment comment
            var assignmentComment = new Comment
            {
                ComplaintId = id,
                AuthorId = newAssignment.AssignedByUserId,
                AuthorName = User.FindFirstValue(ClaimTypes.Name) ?? "Admin",
                AuthorRole = "Admin",
                Content = $"Complaint assigned to department and technician",
                VisibleToUser = true,
                CreatedAt = DateTime.UtcNow
            };

            _db.Comments.Add(assignmentComment);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpGet]
        public async Task<IActionResult> Export(string? status, int? departmentId, int? assignedToUserId, string? q, string? sortBy = "CreatedAt", string? sortOrder = "desc", string format = "csv")
        {
            var query = _db.Complaints
                .Include(c => c.User)
                .Include(c => c.Department)
                .Include(c => c.AssignedToUser)
                .AsQueryable();

            // Apply same filters as Index
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<ComplaintStatus>(status, true, out var parsedStatus))
            {
                query = query.Where(c => c.Status == parsedStatus);
            }

            if (departmentId.HasValue)
            {
                query = query.Where(c => c.DepartmentId == departmentId.Value);
            }

            if (assignedToUserId.HasValue)
            {
                query = query.Where(c => c.AssignedToUserId == assignedToUserId.Value);
            }

            if (!string.IsNullOrEmpty(q))
            {
                query = query.Where(c => c.ProblemType.Contains(q) || 
                                       (c.Description != null && c.Description.Contains(q)) ||
                                       (c.Address != null && c.Address.Contains(q)) ||
                                       c.User!.Name.Contains(q));
            }

            // Apply sorting
            query = sortBy.ToLower() switch
            {
                "createdat" => sortOrder == "asc" ? query.OrderBy(c => c.CreatedAt) : query.OrderByDescending(c => c.CreatedAt),
                "status" => sortOrder == "asc" ? query.OrderBy(c => c.Status) : query.OrderByDescending(c => c.Status),
                "problemtype" => sortOrder == "asc" ? query.OrderBy(c => c.ProblemType) : query.OrderByDescending(c => c.ProblemType),
                "username" => sortOrder == "asc" ? query.OrderBy(c => c.User!.Name) : query.OrderByDescending(c => c.User!.Name),
                "department" => sortOrder == "asc" ? query.OrderBy(c => c.Department!.Name) : query.OrderByDescending(c => c.Department!.Name),
                _ => query.OrderByDescending(c => c.CreatedAt)
            };

            var complaints = await query.ToListAsync();

            return format.ToLower() switch
            {
                "csv" => await ExportToCsv(complaints),
                "excel" => await ExportToExcel(complaints),
                "pdf" => await ExportToPdf(complaints),
                _ => await ExportToCsv(complaints)
            };
        }

        private async Task<IActionResult> ExportToCsv(List<Complaint> complaints)
        {
            var csv = new System.Text.StringBuilder();
            csv.AppendLine("Complaint ID,Status,Problem Type,Description,User,Department,Assigned To,Address,Latitude,Longitude,Created At,Updated At");

            foreach (var complaint in complaints)
            {
                csv.AppendLine($"{complaint.ComplaintId},{complaint.Status},{complaint.ProblemType}," +
                              $"\"{complaint.Description?.Replace("\"", "\"\"")}\"," +
                              $"{complaint.User?.Name},{complaint.Department?.Name},{complaint.AssignedToUser?.Name}," +
                              $"\"{complaint.Address?.Replace("\"", "\"\"")}\"," +
                              $"{complaint.Latitude},{complaint.Longitude}," +
                              $"{complaint.CreatedAt:yyyy-MM-dd HH:mm:ss}," +
                              $"{complaint.UpdatedAt:yyyy-MM-dd HH:mm:ss}");
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", $"complaints_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
        }

        private async Task<IActionResult> ExportToExcel(List<Complaint> complaints)
        {
            // For Excel export, we'll use a simple CSV format with .xlsx extension
            // In a real application, you'd use a library like EPPlus or ClosedXML
            var csv = new System.Text.StringBuilder();
            csv.AppendLine("Complaint ID,Status,Problem Type,Description,User,Department,Assigned To,Address,Latitude,Longitude,Created At,Updated At");

            foreach (var complaint in complaints)
            {
                csv.AppendLine($"{complaint.ComplaintId},{complaint.Status},{complaint.ProblemType}," +
                              $"\"{complaint.Description?.Replace("\"", "\"\"")}\"," +
                              $"{complaint.User?.Name},{complaint.Department?.Name},{complaint.AssignedToUser?.Name}," +
                              $"\"{complaint.Address?.Replace("\"", "\"\"")}\"," +
                              $"{complaint.Latitude},{complaint.Longitude}," +
                              $"{complaint.CreatedAt:yyyy-MM-dd HH:mm:ss}," +
                              $"{complaint.UpdatedAt:yyyy-MM-dd HH:mm:ss}");
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"complaints_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
        }

        private async Task<IActionResult> ExportToPdf(List<Complaint> complaints)
        {
            // For PDF export, we'll return a simple HTML that can be printed as PDF
            // In a real application, you'd use a library like iTextSharp or DinkToPdf
            var html = $@"
                <html>
                <head>
                    <title>Complaints Report</title>
                    <style>
                        body {{ font-family: Arial, sans-serif; margin: 20px; }}
                        table {{ border-collapse: collapse; width: 100%; }}
                        th, td {{ border: 1px solid #ddd; padding: 8px; text-align: left; }}
                        th {{ background-color: #f2f2f2; }}
                        .header {{ text-align: center; margin-bottom: 20px; }}
                    </style>
                </head>
                <body>
                    <div class='header'>
                        <h1>Complaints Report</h1>
                        <p>Generated on {DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>
                        <p>Total Complaints: {complaints.Count}</p>
                    </div>
                    <table>
                        <tr>
                            <th>ID</th><th>Status</th><th>Type</th><th>User</th><th>Department</th><th>Created</th>
                        </tr>";

            foreach (var complaint in complaints)
            {
                html += $@"
                        <tr>
                            <td>{complaint.ComplaintId}</td>
                            <td>{complaint.Status}</td>
                            <td>{complaint.ProblemType}</td>
                            <td>{complaint.User?.Name}</td>
                            <td>{complaint.Department?.Name}</td>
                            <td>{complaint.CreatedAt:yyyy-MM-dd}</td>
                        </tr>";
            }

            html += @"
                    </table>
                </body>
                </html>";

            var bytes = System.Text.Encoding.UTF8.GetBytes(html);
            return File(bytes, "text/html", $"complaints_report_{DateTime.Now:yyyyMMdd_HHmmss}.html");
        }

        [HttpGet]
        public async Task<IActionResult> GetTechniciansByDepartment(int departmentId)
        {
            var technicians = await _db.Users
                .Where(u => u.Role == "Technician" && u.DepartmentId == departmentId)
                .Select(u => new { u.UserId, u.Name, u.Email })
                .ToListAsync();

            return Json(technicians);
        }

        [HttpGet]
        public IActionResult CreateTechnician()
        {
            ViewBag.Departments = _db.Departments.Where(d => d.IsActive).ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateTechnician(string name, string email, string password, int departmentId)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                TempData["ErrorMessage"] = "All fields are required.";
                ViewBag.Departments = await _db.Departments.Where(d => d.IsActive).ToListAsync();
                return View();
            }

            if (await _db.Users.AnyAsync(u => u.Email == email))
            {
                TempData["ErrorMessage"] = "Email already exists.";
                ViewBag.Departments = await _db.Departments.Where(d => d.IsActive).ToListAsync();
                return View();
            }

            var technician = new User
            {
                Name = name,
                Email = email,
                PasswordHash = _passwordService.HashPassword(password),
                Points = 0,
                BadgeLevel = "Bronze",
                Role = "Technician",
                DepartmentId = departmentId
            };

            _db.Users.Add(technician);
            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = "Technician created successfully.";
            return RedirectToAction("Index", "Role");
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteComplaint(int id)
        {
            try
            {
                var complaint = await _db.Complaints.FindAsync(id);
                if (complaint == null)
                {
                    return Json(new { success = false, message = "Complaint not found" });
                }

                // Remove related data
                var comments = await _db.Comments.Where(c => c.ComplaintId == id).ToListAsync();
                var ratings = await _db.ComplaintRatings.Where(r => r.ComplaintId == id).ToListAsync();
                var assignments = await _db.ComplaintAssignments.Where(a => a.ComplaintId == id).ToListAsync();

                _db.Comments.RemoveRange(comments);
                _db.ComplaintRatings.RemoveRange(ratings);
                _db.ComplaintAssignments.RemoveRange(assignments);
                _db.Complaints.Remove(complaint);

                await _db.SaveChangesAsync();

                return Json(new { success = true, message = "Complaint deleted successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error deleting complaint: " + ex.Message });
            }
        }
    }
}


