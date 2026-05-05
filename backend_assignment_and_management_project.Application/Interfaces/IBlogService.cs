using backend_assignment_and_management_project.Application.DTOs;

namespace backend_assignment_and_management_project.Application.Interfaces
{
    public interface IBlogService
    {
        Task<IEnumerable<SubjectDto>> GetSubjectsAsync();
        Task<IEnumerable<PostDto>> GetPostsAsync(Guid currentUserId, int page = 1, int pageSize = 10, Guid? subjectId = null);
        Task<PostDto?> GetPostByIdAsync(Guid currentUserId, Guid postId);
        Task<PostDto> CreatePostAsync(Guid userId, CreatePostDto dto);
        Task<PostDto?> UpdatePostAsync(Guid userId, Guid postId, UpdatePostDto dto);
        Task<bool> DeletePostAsync(Guid userId, Guid postId);
        Task<bool> LikePostAsync(Guid userId, Guid postId);
        Task<bool> UnlikePostAsync(Guid userId, Guid postId);
        Task<IEnumerable<CommentDto>> GetCommentsAsync(Guid postId);
        Task<CommentDto> AddCommentAsync(Guid userId, Guid postId, CreateCommentDto dto);
        Task<bool> DeleteCommentAsync(Guid userId, Guid commentId);
        Task<bool> UpdateUserSubjectsAsync(Guid userId, List<Guid> subjectIds);
        Task<IEnumerable<SubjectDto>> GetUserSubjectsAsync(Guid userId);
    }
}
