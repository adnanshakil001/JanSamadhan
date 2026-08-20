using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JanSamadhan.Models
{
    public class Department
    {
        [Key]
        public int DepartmentId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required, MaxLength(50)]
        public string Email { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public ICollection<ProblemTypeMapping> ProblemTypeMappings { get; set; } = new List<ProblemTypeMapping>();
        public ICollection<Complaint> AssignedComplaints { get; set; } = new List<Complaint>();
    }

    public class ProblemTypeMapping
    {
        [Key]
        public int MappingId { get; set; }

        [Required, MaxLength(50)]
        public string ProblemType { get; set; } = string.Empty;

        [Required]
        public int DepartmentId { get; set; }

        [ForeignKey(nameof(DepartmentId))]
        public Department? Department { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
