using Microsoft.EntityFrameworkCore;
using backend_assignment_and_deadline_management_project.Domain.Entities;

namespace backend_assignment_and_deadline_management_project.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<StudyFile> StudyFiles { get; set; }
        public DbSet<StudyLog> StudyLogs { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<UserActivityLog> UserActivityLogs { get; set; }
        public DbSet<SystemLog> SystemLogs { get; set; }
        public DbSet<UserSubject> UserSubjects { get; set; }
        public DbSet<PostLike> PostLikes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserSubject>()
                .HasKey(us => new { us.UserId, us.SubjectId });

            modelBuilder.Entity<PostLike>()
                .HasKey(pl => new { pl.PostId, pl.UserId });

            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entity.GetTableName();
                if (!string.IsNullOrEmpty(tableName))
                {
                    entity.SetTableName(tableName.ToLower());
                }

                foreach (var property in entity.GetProperties())
                {
                    property.SetColumnName(System.Text.RegularExpressions.Regex.Replace(
                        property.Name, "([a-z])([A-Z])", "$1_$2").ToLower());
                }
            }
        }
    }
}