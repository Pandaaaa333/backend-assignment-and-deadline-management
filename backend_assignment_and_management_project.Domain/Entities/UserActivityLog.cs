using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend_assignment_and_deadline_management_project.Domain.Entities
{
    [Table("user_activity_log")]
    public class UserActivityLog
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column("action")]
        public string Action { get; set; } = string.Empty;

        [Column("target_entity")]
        public string? TargetEntity { get; set; }

        [Column("target_id")]
        public Guid? TargetId { get; set; }

        [Column("data_change")]
        public string? DataChange { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("user_id")]
        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;
    }
}