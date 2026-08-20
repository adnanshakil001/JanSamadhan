using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JanSamadhan.Models
{
    public class TechnicianPhoto
    {
        [Key]
        public int PhotoId { get; set; }

        [Required]
        public int ComplaintId { get; set; }

        [ForeignKey(nameof(ComplaintId))]
        public Complaint Complaint { get; set; } = null!;

        [Required, MaxLength(50)]
        public string PhotoType { get; set; } = string.Empty; // "WorkInProgress" or "Fixed"

        [Required, MaxLength(500)]
        public string PhotoPath { get; set; } = string.Empty;

        public DateTime UploadedAt { get; set; }
    }
}
