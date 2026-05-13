using System.Security.Claims;
using backend_assignment_and_management_project.Application.DTOs;
using backend_assignment_and_management_project.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace backend_assignment_and_management_project.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PostsController : ControllerBase
    {
        private readonly IBlogService _blogService;
        private readonly IStorageService _storageService;

        public PostsController(IBlogService blogService, IStorageService storageService)
        {
            _blogService = blogService;
            _storageService = storageService;
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) throw new UnauthorizedAccessException();
            return Guid.Parse(userIdClaim.Value);
        }

        [HttpGet]
        public async Task<IActionResult> GetPosts([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] Guid? subjectId = null)
        {
            var currentUserId = GetCurrentUserId();
            var posts = await _blogService.GetPostsAsync(currentUserId, page, pageSize, subjectId);
            return Ok(posts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPostById(Guid id)
        {
            var currentUserId = GetCurrentUserId();
            var post = await _blogService.GetPostByIdAsync(currentUserId, id);
            if (post == null) return NotFound();
            return Ok(post);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostDto dto)
        {
            var userId = GetCurrentUserId();
            var post = await _blogService.CreatePostAsync(userId, dto);
            return CreatedAtAction(nameof(GetPostById), new { id = post.Id }, post);
        }

        [HttpPost("with-image")]
        public async Task<IActionResult> CreatePostWithImage([FromForm] string content, [FromForm] Guid subjectId, IFormFile? image)
        {
            try
            {
                var userId = GetCurrentUserId();
                string? imageUrl = null;
                if (image != null && image.Length > 0)
                {
                    imageUrl = await _storageService.UploadFileAsync(image.OpenReadStream(), image.FileName, image.ContentType, "posts");
                }
                
                var dto = new CreatePostDto
                {
                    Content = content,
                    SubjectId = subjectId,
                    ImageUrl = imageUrl
                };
                
                var post = await _blogService.CreatePostAsync(userId, dto);
                return CreatedAtAction(nameof(GetPostById), new { id = post.Id }, post);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePost(Guid id, [FromBody] UpdatePostDto dto)
        {
            var userId = GetCurrentUserId();
            var post = await _blogService.UpdatePostAsync(userId, id, dto);
            if (post == null) return NotFound("Post not found or you are not the author.");
            return Ok(post);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(Guid id)
        {
            var userId = GetCurrentUserId();
            var result = await _blogService.DeletePostAsync(userId, id);
            if (!result) return NotFound("Post not found or you are not the author.");
            return NoContent();
        }

        [HttpPost("{id}/like")]
        public async Task<IActionResult> LikePost(Guid id)
        {
            var userId = GetCurrentUserId();
            await _blogService.LikePostAsync(userId, id);
            return Ok();
        }

        [HttpPost("{id}/unlike")]
        public async Task<IActionResult> UnlikePost(Guid id)
        {
            var userId = GetCurrentUserId();
            await _blogService.UnlikePostAsync(userId, id);
            return Ok();
        }

        [HttpGet("{id}/comments")]
        public async Task<IActionResult> GetComments(Guid id)
        {
            var comments = await _blogService.GetCommentsAsync(id);
            return Ok(comments);
        }

        [HttpPost("{id}/comments")]
        public async Task<IActionResult> AddComment(Guid id, [FromBody] CreateCommentDto dto)
        {
            var userId = GetCurrentUserId();
            var comment = await _blogService.AddCommentAsync(userId, id, dto);
            return Ok(comment);
        }

        [HttpDelete("comments/{commentId}")]
        public async Task<IActionResult> DeleteComment(Guid commentId)
        {
            var userId = GetCurrentUserId();
            var result = await _blogService.DeleteCommentAsync(userId, commentId);
            if (!result) return NotFound("Comment not found or you are not the author.");
            return NoContent();
        }
    }
}
