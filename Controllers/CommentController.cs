using System.Security.Claims;
using JanSamadhan.Data;
using JanSamadhan.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JanSamadhan.Controllers
{
    [Authorize]
    public class CommentController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;

        public CommentController(ApplicationDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        [HttpPost]
        public async Task<IActionResult> Create(int complaintId, string content, bool visibleToUser = true, IFormFileCollection? attachments = null)
        {
            if (string.IsNullOrWhiteSpace(content) || content.Length > 1000)
            {
                return Json(new { success = false, message = "Content must be between 1 and 1000 characters." });
            }

            var complaint = await _db.Complaints.FindAsync(complaintId);
            if (complaint == null)
            {
                return Json(new { success = false, message = "Complaint not found." });
            }

            // Check if user has permission to comment
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var userRole = User.FindFirstValue(ClaimTypes.Role) ?? "User";
            
            if (userRole == "User" && complaint.UserId != userId)
            {
                return Json(new { success = false, message = "You can only comment on your own complaints." });
            }

            var comment = new Comment
            {
                ComplaintId = complaintId,
                AuthorId = userId,
                AuthorName = User.FindFirstValue(ClaimTypes.Name) ?? "Unknown",
                AuthorRole = userRole,
                Content = content.Trim(),
                VisibleToUser = visibleToUser || userRole == "User", // Users' comments are always visible
                CreatedAt = DateTime.UtcNow
            };

            _db.Comments.Add(comment);
            await _db.SaveChangesAsync();

            // Handle attachments
            if (attachments != null && attachments.Count > 0)
            {
                var uploadsPath = Path.Combine(_env.WebRootPath, "uploads", "comments");
                Directory.CreateDirectory(uploadsPath);

                foreach (var attachment in attachments.Take(3)) // Max 3 attachments
                {
                    if (attachment.Length > 0 && IsValidFileType(attachment))
                    {
                        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(attachment.FileName)}";
                        var filePath = Path.Combine(uploadsPath, fileName);
                        
                        using (var stream = System.IO.File.Create(filePath))
                        {
                            await attachment.CopyToAsync(stream);
                        }

                        var commentAttachment = new CommentAttachment
                        {
                            CommentId = comment.CommentId,
                            FileName = attachment.FileName,
                            FilePath = $"/uploads/comments/{fileName}",
                            ContentType = attachment.ContentType,
                            FileSize = attachment.Length,
                            UploadedAt = DateTime.UtcNow
                        };

                        _db.CommentAttachments.Add(commentAttachment);
                    }
                }
            }

            await _db.SaveChangesAsync();

            return Json(new { success = true, commentId = comment.CommentId });
        }

        [HttpGet]
        public async Task<IActionResult> GetComments(int complaintId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var userRole = User.FindFirstValue(ClaimTypes.Role) ?? "User";
            
            var complaint = await _db.Complaints.FindAsync(complaintId);
            if (complaint == null)
            {
                return Json(new { success = false, message = "Complaint not found." });
            }

            // Check permissions
            if (userRole == "User" && complaint.UserId != userId)
            {
                return Json(new { success = false, message = "Access denied." });
            }

            var comments = await _db.Comments
                .Where(c => c.ComplaintId == complaintId)
                .Where(c => userRole != "User" || c.VisibleToUser) // Users only see visible comments
                .Include(c => c.Attachments)
                .OrderBy(c => c.CreatedAt)
                .Select(c => new
                {
                    c.CommentId,
                    c.AuthorName,
                    c.AuthorRole,
                    c.Content,
                    c.VisibleToUser,
                    c.CreatedAt,
                    Attachments = c.Attachments.Select(a => new
                    {
                        a.AttachmentId,
                        a.FileName,
                        a.FilePath,
                        a.ContentType,
                        a.FileSize
                    })
                })
                .ToListAsync();

            return Json(new { success = true, comments });
        }

        private bool IsValidFileType(IFormFile file)
        {
            var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "application/pdf" };
            return allowedTypes.Contains(file.ContentType.ToLower());
        }
    }
}
