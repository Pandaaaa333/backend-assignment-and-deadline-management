using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend_assignment_and_deadline_management_project.Domain.Entities
{
    [Table("system_log")]
    public class SystemLog
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column("log_level")]
        public string? LogLevel { get; set; }

        [Column("message")]
        public string Message { get; set; } = string.Empty;

        [Column("stack_trace")]
        public string? StackTrace { get; set; }

        [Column("request_paths")]
        public string? RequestPaths { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}