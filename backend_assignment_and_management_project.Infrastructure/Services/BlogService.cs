using backend_assignment_and_management_project.Application.DTOs;
using backend_assignment_and_management_project.Application.Interfaces;
using backend_assignment_and_management_project.Domain.Entities;
using backend_assignment_and_management_project.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace backend_assignment_and_management_project.Infrastructure.Services
{
    public class BlogService : IBlogService
    {
        private readonly ApplicationDbContext _context;
        private readonly IStorageService _storageService;

        public BlogService(ApplicationDbContext context, IStorageService storageService)
        {
            _context = context;
            _storageService = storageService;
        }

        public async Task<IEnumerable<SubjectDto>> GetSubjectsAsync()
        {
            return await _context.Subjects
                .Select(s => new SubjectDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<PostDto>> GetPostsAsync(Guid currentUserId, int page = 1, int pageSize = 10, Guid? subjectId = null)
        {
            var query = _context.Posts
                .Include(p => p.User)
                .Include(p => p.Subject)
                .AsQueryable();

            if (subjectId.HasValue)
            {
                query = query.Where(p => p.SubjectId == subjectId.Value);
            }
            else
            {
                // If no specific subject, filter by user's selected subjects
                var userSubjectIds = await _context.UserSubjects
                    .Where(us => us.UserId == currentUserId)
                    .Select(us => us.SubjectId)
                    .ToListAsync();
                
                if (userSubjectIds.Any())
                {
                    query = query.Where(p => userSubjectIds.Contains(p.SubjectId));
                }
            }

            var posts = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var postDtos = new List<PostDto>();

            foreach (var post in posts)
            {
                var likeCount = await _context.PostLikes.CountAsync(l => l.PostId == post.Id);
                var commentCount = await _context.Comments.CountAsync(c => c.PostId == post.Id);
                var isLiked = await _context.PostLikes.AnyAsync(l => l.PostId == post.Id && l.UserId == currentUserId);

                postDtos.Add(new PostDto
                {
                    Id = post.Id,
                    Content = post.Content,
                    ImageUrl = !string.IsNullOrEmpty(post.ImageUrl) ? await _storageService.GetPresignedUrlAsync(post.ImageUrl) : null,
                    CreatedAt = post.CreatedAt,
                    UserId = post.UserId,
                    UserName = post.User.Name,
                    UserAvatar = !string.IsNullOrEmpty(post.User.AvatarUrl) ? await _storageService.GetPresignedUrlAsync(post.User.AvatarUrl) : null,
                    SubjectId = post.SubjectId,
                    SubjectName = post.Subject.Name,
                    LikeCount = likeCount,
                    CommentCount = commentCount,
                    IsLiked = isLiked
                });
            }

            return postDtos;
        }

        public async Task<PostDto?> GetPostByIdAsync(Guid currentUserId, Guid postId)
        {
            var post = await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Subject)
                .FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null) return null;

            var likeCount = await _context.PostLikes.CountAsync(l => l.PostId == post.Id);
            var commentCount = await _context.Comments.CountAsync(c => c.PostId == post.Id);
            var isLiked = await _context.PostLikes.AnyAsync(l => l.PostId == post.Id && l.UserId == currentUserId);

            return new PostDto
            {
                Id = post.Id,
                Content = post.Content,
                ImageUrl = !string.IsNullOrEmpty(post.ImageUrl) ? await _storageService.GetPresignedUrlAsync(post.ImageUrl) : null,
                CreatedAt = post.CreatedAt,
                UserId = post.UserId,
                UserName = post.User.Name,
                UserAvatar = !string.IsNullOrEmpty(post.User.AvatarUrl) ? await _storageService.GetPresignedUrlAsync(post.User.AvatarUrl) : null,
                SubjectId = post.SubjectId,
                SubjectName = post.Subject.Name,
                LikeCount = likeCount,
                CommentCount = commentCount,
                IsLiked = isLiked
            };
        }

        public async Task<PostDto> CreatePostAsync(Guid userId, CreatePostDto dto)
        {
            var post = new Post
            {
                Content = dto.Content,
                ImageUrl = dto.ImageUrl,
                UserId = userId,
                SubjectId = dto.SubjectId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            // Fetch with includes for the DTO
            return (await GetPostByIdAsync(userId, post.Id))!;
        }

        public async Task<PostDto?> UpdatePostAsync(Guid userId, Guid postId, UpdatePostDto dto)
        {
            var post = await _context.Posts.FindAsync(postId);

            if (post == null || post.UserId != userId) return null;

            post.Content = dto.Content;
            post.ImageUrl = dto.ImageUrl;
            post.SubjectId = dto.SubjectId;

            _context.Posts.Update(post);
            await _context.SaveChangesAsync();

            return await GetPostByIdAsync(userId, post.Id);
        }

        public async Task<bool> DeletePostAsync(Guid userId, Guid postId)
        {
            var post = await _context.Posts.FindAsync(postId);

            if (post == null || post.UserId != userId) return false;

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> LikePostAsync(Guid userId, Guid postId)
        {
            if (await _context.PostLikes.AnyAsync(l => l.PostId == postId && l.UserId == userId))
                return true; // Already liked

            var like = new PostLike
            {
                PostId = postId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.PostLikes.Add(like);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnlikePostAsync(Guid userId, Guid postId)
        {
            var like = await _context.PostLikes.FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId);

            if (like == null) return true; // Already unliked

            _context.PostLikes.Remove(like);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<CommentDto>> GetCommentsAsync(Guid postId)
        {
            var comments = await _context.Comments
                .Include(c => c.User)
                .Where(c => c.PostId == postId)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    UserId = c.UserId,
                    UserName = c.User.Name,
                    UserAvatar = c.User.AvatarUrl
                })
                .ToListAsync();
            
            // Resolve avatars for comments
            foreach (var comment in comments)
            {
                if (!string.IsNullOrEmpty(comment.UserAvatar))
                {
                    comment.UserAvatar = await _storageService.GetPresignedUrlAsync(comment.UserAvatar);
                }
            }
            return comments;
        }

        public async Task<CommentDto> AddCommentAsync(Guid userId, Guid postId, CreateCommentDto dto)
        {
            var comment = new Comment
            {
                Content = dto.Content,
                PostId = postId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            var user = await _context.Users.FindAsync(userId);

            return new CommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                UserId = userId,
                UserName = user?.Name ?? "Unknown",
                UserAvatar = !string.IsNullOrEmpty(user?.AvatarUrl) ? await _storageService.GetPresignedUrlAsync(user.AvatarUrl) : null
            };
        }

        public async Task<bool> DeleteCommentAsync(Guid userId, Guid commentId)
        {
            var comment = await _context.Comments.FindAsync(commentId);

            if (comment == null || comment.UserId != userId) return false;

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<SubjectDto>> GetUserSubjectsAsync(Guid userId)
        {
            return await _context.UserSubjects
                .Where(us => us.UserId == userId)
                .Include(us => us.Subject)
                .Select(us => new SubjectDto
                {
                    Id = us.SubjectId,
                    Name = us.Subject.Name,
                    Description = us.Subject.Description
                })
                .ToListAsync();
        }

        public async Task<bool> UpdateUserSubjectsAsync(Guid userId, List<Guid> subjectIds)
        {
            // Remove existing preferences
            var existing = await _context.UserSubjects.Where(us => us.UserId == userId).ToListAsync();
            _context.UserSubjects.RemoveRange(existing);

            // Add new preferences
            var newPreferences = subjectIds.Select(sid => new UserSubject
            {
                UserId = userId,
                SubjectId = sid,
                JoinedAt = DateTime.UtcNow
            });

            await _context.UserSubjects.AddRangeAsync(newPreferences);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
