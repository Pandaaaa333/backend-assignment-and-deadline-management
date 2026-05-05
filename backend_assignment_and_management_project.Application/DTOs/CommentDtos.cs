using System.ComponentModel.DataAnnotations;

namespace backend_assignment_and_management_project.Application.DTOs
{
    public class CommentDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? UserAvatar { get; set; }
    }

    public class CreateCommentDto
    {
        [Required]
        public string Content { get; set; } = string.Empty;
    }
}
