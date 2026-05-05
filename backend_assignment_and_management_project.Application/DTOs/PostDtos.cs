using System.ComponentModel.DataAnnotations;

namespace backend_assignment_and_management_project.Application.DTOs
{
    public class PostDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? UserAvatar { get; set; }
        public Guid SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public bool IsLiked { get; set; }
    }

    public class CreatePostDto
    {
        [Required]
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        [Required]
        public Guid SubjectId { get; set; }
    }

    public class UpdatePostDto
    {
        [Required]
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public Guid SubjectId { get; set; }
    }
}
