using System.ComponentModel.DataAnnotations;

namespace JanSamadhan.Models
{
    public class Badge
    {
        [Key]
        public int BadgeId { get; set; }

        [Required, MaxLength(50)]
        public string LevelName { get; set; } = string.Empty;

        [Range(0, int.MaxValue)]
        public int PointsRequired { get; set; }
    }
}


