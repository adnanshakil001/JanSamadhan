using JanSamadhan.Models;
using Microsoft.EntityFrameworkCore;

namespace JanSamadhan.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Admin> Admins => Set<Admin>();
        public DbSet<Complaint> Complaints => Set<Complaint>();
        public DbSet<Badge> Badges => Set<Badge>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<CommentAttachment> CommentAttachments => Set<CommentAttachment>();
        public DbSet<ComplaintRating> ComplaintRatings => Set<ComplaintRating>();
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<ProblemTypeMapping> ProblemTypeMappings => Set<ProblemTypeMapping>();
        public DbSet<ComplaintAssignment> ComplaintAssignments => Set<ComplaintAssignment>();
        public DbSet<TechnicianPhoto> TechnicianPhotos => Set<TechnicianPhoto>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Complaints)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.AssignedComplaints)
                .WithOne(c => c.AssignedToUser)
                .HasForeignKey(c => c.AssignedToUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Complaint>()
                .HasMany(c => c.Comments)
                .WithOne(co => co.Complaint)
                .HasForeignKey(co => co.ComplaintId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>()
                .HasMany(c => c.Attachments)
                .WithOne(a => a.Comment)
                .HasForeignKey(a => a.CommentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Complaint>()
                .HasOne(c => c.Rating)
                .WithOne(r => r.Complaint)
                .HasForeignKey<ComplaintRating>(r => r.ComplaintId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ComplaintRating>()
                .HasOne(r => r.User)
                .WithMany(u => u.Ratings)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Department>()
                .HasMany(d => d.ProblemTypeMappings)
                .WithOne(m => m.Department)
                .HasForeignKey(m => m.DepartmentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Department>()
                .HasMany(d => d.AssignedComplaints)
                .WithOne(c => c.Department)
                .HasForeignKey(c => c.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Complaint>()
                .HasMany(c => c.AssignmentHistory)
                .WithOne(a => a.Complaint)
                .HasForeignKey(a => a.ComplaintId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ComplaintAssignment>()
                .HasOne(a => a.AssignedByUser)
                .WithMany()
                .HasForeignKey(a => a.AssignedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TechnicianPhoto>()
                .HasOne(t => t.Complaint)
                .WithMany(c => c.TechnicianPhotos)
                .HasForeignKey(t => t.ComplaintId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Badge>().HasData(
                new Badge { BadgeId = 1, LevelName = "Bronze", PointsRequired = 10 },
                new Badge { BadgeId = 2, LevelName = "Silver", PointsRequired = 30 },
                new Badge { BadgeId = 3, LevelName = "Gold", PointsRequired = 60 },
                new Badge { BadgeId = 4, LevelName = "Platinum", PointsRequired = 100 },
                new Badge { BadgeId = 5, LevelName = "Diamond", PointsRequired = 150 }
            );

            // Seed departments
            modelBuilder.Entity<Department>().HasData(
                new Department { DepartmentId = 1, Name = "Public Works", Email = "publicworks@city.gov", Description = "Handles infrastructure issues" },
                new Department { DepartmentId = 2, Name = "Traffic Management", Email = "traffic@city.gov", Description = "Manages traffic signals and road safety" },
                new Department { DepartmentId = 3, Name = "Utilities", Email = "utilities@city.gov", Description = "Water, sewer, and electrical issues" }
            );

            // Seed problem type mappings
            modelBuilder.Entity<ProblemTypeMapping>().HasData(
                new ProblemTypeMapping { MappingId = 1, ProblemType = "Pothole", DepartmentId = 1 },
                new ProblemTypeMapping { MappingId = 2, ProblemType = "Streetlight", DepartmentId = 1 },
                new ProblemTypeMapping { MappingId = 3, ProblemType = "Traffic Signal", DepartmentId = 2 },
                new ProblemTypeMapping { MappingId = 4, ProblemType = "Water Disposal", DepartmentId = 3 },
                new ProblemTypeMapping { MappingId = 5, ProblemType = "Sewer Lids", DepartmentId = 3 },
                new ProblemTypeMapping { MappingId = 6, ProblemType = "Bridges", DepartmentId = 1 }
            );

        }
    }
}


