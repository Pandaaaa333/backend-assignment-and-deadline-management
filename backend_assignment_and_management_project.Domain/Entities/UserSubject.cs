using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend_assignment_and_deadline_management_project.Domain.Entities
{
    [Table("users_subject")]
    public class UserSubject
    {
        [Column("user_id")]
        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        [Column("subject_id")]
        public Guid SubjectId { get; set; }

        [ForeignKey("SubjectId")]
        public Subject Subject { get; set; } = null!;

        [Column("joined_at")]
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    }
}