using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend_assignment_and_deadline_management_project.Domain.Entities
{
    [Table("study_files")]
    public class StudyFile
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [Column("file_name")]
        [MaxLength(255)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        [Column("file_url")]
        public string FileUrl { get; set; } = string.Empty;

        [Column("file_type")]
        [MaxLength(50)]
        public string? FileType { get; set; }

        [Column("uploaded_at")]
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        [Column("user_id")]
        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        [Column("subject_id")]
        public Guid SubjectId { get; set; }

        [ForeignKey("SubjectId")]
        public Subject Subject { get; set; } = null!;
    }
}