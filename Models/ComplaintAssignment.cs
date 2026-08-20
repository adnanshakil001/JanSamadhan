using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JanSamadhan.Models
{
    public class ComplaintAssignment
    {
        [Key]
        public int AssignmentId { get; set; }

        [Required]
        public int ComplaintId { get; set; }

        [ForeignKey(nameof(ComplaintId))]
        public Complaint? Complaint { get; set; }

        public int? DepartmentId { get; set; }

        [ForeignKey(nameof(DepartmentId))]
        public Department? Department { get; set; }

        public int? AssignedToUserId { get; set; }

        [ForeignKey(nameof(AssignedToUserId))]
        public User? AssignedToUser { get; set; }

        [MaxLength(500)]
        public string? Note { get; set; }

        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        public int AssignedByUserId { get; set; }

        [ForeignKey(nameof(AssignedByUserId))]
        public User? AssignedByUser { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
