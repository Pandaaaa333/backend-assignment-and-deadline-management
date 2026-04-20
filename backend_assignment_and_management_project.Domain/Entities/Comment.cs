using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend_assignment_and_deadline_management_project.Domain.Entities
{
    [Table("comments")]
    public class Comment
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [Column("content")]
        public string Content { get; set; } = string.Empty;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("post_id")]
        public Guid PostId { get; set; }

        [ForeignKey("PostId")]
        public Post Post { get; set; } = null!;

        [Column("user_id")]
        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;
    }
}