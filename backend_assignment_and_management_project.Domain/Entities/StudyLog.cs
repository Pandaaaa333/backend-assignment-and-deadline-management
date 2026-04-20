using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend_assignment_and_deadline_management_project.Domain.Entities
{
    [Table("study_logs")]
    public class StudyLog
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column("start_time")]
        public DateTime StartTime { get; set; }

        [Column("end_time")]
        public DateTime EndTime { get; set; }

        [Column("duration")]
        public int Duration { get; set; }

        [Column("note")]
        public string? Note { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        [Column("file_id")]
        public Guid? FileId { get; set; }

        [ForeignKey("FileId")]
        public StudyFile? StudyFile { get; set; }
    }
}