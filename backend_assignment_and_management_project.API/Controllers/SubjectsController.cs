using backend_assignment_and_management_project.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace backend_assignment_and_management_project.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SubjectsController : ControllerBase
    {
        private readonly IBlogService _blogService;

        public SubjectsController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        [HttpGet]
        public async Task<IActionResult> GetSubjects()
        {
            var subjects = await _blogService.GetSubjectsAsync();
            return Ok(subjects);
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMySubjects()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            
            var subjects = await _blogService.GetUserSubjectsAsync(Guid.Parse(userIdClaim.Value));
            return Ok(subjects);
        }

        [HttpPost("preferences")]
        public async Task<IActionResult> UpdatePreferences([FromBody] List<Guid> subjectIds)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            await _blogService.UpdateUserSubjectsAsync(Guid.Parse(userIdClaim.Value), subjectIds);
            return Ok(new { message = "Cập nhật sở thích thành công" });
        }
    }
}
