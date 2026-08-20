using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JanSamadhan.Models
{
    public enum ComplaintStatus
    {
        Pending = 0,
        InProgress = 1,
        Fixed = 2
    }

    public class Complaint
    {
        [Key]
        public int ComplaintId { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }

        [Required, MaxLength(50)]
        public string ProblemType { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(255)]
        public string? PhotoPath { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        [MaxLength(255)]
        public string? Address { get; set; }

        public ComplaintStatus Status { get; set; } = ComplaintStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public int? DepartmentId { get; set; }

        [ForeignKey(nameof(DepartmentId))]
        public Department? Department { get; set; }

        public int? AssignedToUserId { get; set; }

        [ForeignKey(nameof(AssignedToUserId))]
        public User? AssignedToUser { get; set; }

        public int? RatingId { get; set; }

        [ForeignKey(nameof(RatingId))]
        public ComplaintRating? Rating { get; set; }

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<ComplaintAssignment> AssignmentHistory { get; set; } = new List<ComplaintAssignment>();
        public ICollection<TechnicianPhoto> TechnicianPhotos { get; set; } = new List<TechnicianPhoto>();
    }
}


