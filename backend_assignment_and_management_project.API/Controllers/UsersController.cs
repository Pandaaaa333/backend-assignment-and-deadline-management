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
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IStorageService _storageService;

        public UsersController(IUserService userService, IStorageService storageService)
        {
            _userService = userService;
            _storageService = storageService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
        {
            try
            {
                var response = await _userService.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequest request)
        {
            try
            {
                // Chỉ Admin hoặc chính User đó mới được update
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                var userRoleClaim = User.FindFirst(ClaimTypes.Role);

                if (userIdClaim == null) return Unauthorized();

                var currentUserId = Guid.Parse(userIdClaim.Value);
                var isAdmin = userRoleClaim?.Value == "Admin";

                if (!isAdmin && currentUserId != id)
                {
                    return Forbid();
                }

                var response = await _userService.UpdateAsync(id, request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _userService.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpPatch("{id}/role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeRole(Guid id, [FromBody] ChangeRoleRequest request)
        {
            try
            {
                var response = await _userService.ChangeRoleAsync(id, request.RoleName);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Unauthorized();
                }

                var userId = Guid.Parse(userIdClaim.Value);
                var response = await _userService.UpdateProfileAsync(userId, request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("avatar")]
        public async Task<IActionResult> UploadAvatar(IFormFile file)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null) return Unauthorized();
                var userId = Guid.Parse(userIdClaim.Value);

                if (file == null || file.Length == 0) return BadRequest("No file uploaded.");

                var filePath = await _storageService.UploadFileAsync(file.OpenReadStream(), file.FileName, file.ContentType, "avatars");
                
                // 2. Update User in DB with the filename/path
                var user = await _userService.GetByIdAsync(userId);
                if (user == null) return NotFound("User not found.");

                // We need a method in UserService to update ONLY the avatar URL
                // Or we can use UpdateProfileAsync if it supports avatar
                await _userService.UpdateProfileAsync(userId, new UpdateProfileRequest 
                { 
                    Name = user.Name,
                    AvatarUrl = filePath 
                });

                // 3. Return the presigned URL for immediate display
                var presignedUrl = await _storageService.GetPresignedUrlAsync(filePath);
                return Ok(new { url = presignedUrl, filePath = filePath });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
