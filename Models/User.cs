using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JanSamadhan.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public int Points { get; set; }

        [MaxLength(50)]
        public string? BadgeLevel { get; set; }

        [MaxLength(20)]
        public string Role { get; set; } = "User";

        public int? DepartmentId { get; set; }

        [ForeignKey(nameof(DepartmentId))]
        public Department? Department { get; set; }

        public ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();
        public ICollection<Complaint> AssignedComplaints { get; set; } = new List<Complaint>();
        public ICollection<ComplaintRating> Ratings { get; set; } = new List<ComplaintRating>();
        public ICollection<ComplaintAssignment> AssignmentHistory { get; set; } = new List<ComplaintAssignment>();
    }
}


