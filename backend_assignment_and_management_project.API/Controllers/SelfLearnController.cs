using System.Security.Claims;
using backend_assignment_and_management_project.Application.DTOs;
using backend_assignment_and_management_project.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend_assignment_and_management_project.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SelfLearnController : ControllerBase
    {
        private readonly ISelfLearnService _selfLearnService;

        public SelfLearnController(ISelfLearnService selfLearnService)
        {
            _selfLearnService = selfLearnService;
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) throw new UnauthorizedAccessException();
            return Guid.Parse(userIdClaim.Value);
        }

        #region Study Logs

        [HttpGet("logs")]
        public async Task<IActionResult> GetStudyLogs([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userId = GetCurrentUserId();
            var logs = await _selfLearnService.GetStudyLogsAsync(userId, page, pageSize);
            return Ok(logs);
        }

        [HttpGet("logs/{id}")]
        public async Task<IActionResult> GetStudyLogById(Guid id)
        {
            var userId = GetCurrentUserId();
            var log = await _selfLearnService.GetStudyLogByIdAsync(userId, id);
            if (log == null) return NotFound();
            return Ok(log);
        }

        [HttpPost("logs")]
        public async Task<IActionResult> CreateStudyLog([FromBody] CreateStudyLogDto dto)
        {
            var userId = GetCurrentUserId();
            var log = await _selfLearnService.CreateStudyLogAsync(userId, dto);
            return CreatedAtAction(nameof(GetStudyLogById), new { id = log.Id }, log);
        }

        [HttpPut("logs/{id}")]
        public async Task<IActionResult> UpdateStudyLog(Guid id, [FromBody] UpdateStudyLogDto dto)
        {
            var userId = GetCurrentUserId();
            var log = await _selfLearnService.UpdateStudyLogAsync(userId, id, dto);
            if (log == null) return NotFound();
            return Ok(log);
        }

        [HttpDelete("logs/{id}")]
        public async Task<IActionResult> DeleteStudyLog(Guid id)
        {
            var userId = GetCurrentUserId();
            var result = await _selfLearnService.DeleteStudyLogAsync(userId, id);
            if (!result) return NotFound();
            return NoContent();
        }

        #endregion

        #region Study Files

        [HttpGet("files")]
        public async Task<IActionResult> GetStudyFiles([FromQuery] Guid? subjectId = null)
        {
            var userId = GetCurrentUserId();
            var files = await _selfLearnService.GetStudyFilesAsync(userId, subjectId);
            return Ok(files);
        }

        [HttpPost("files")]
        public async Task<IActionResult> UploadStudyFile([FromBody] UploadStudyFileDto dto)
        {
            var userId = GetCurrentUserId();
            var file = await _selfLearnService.UploadStudyFileAsync(userId, dto);
            return Ok(file);
        }

        [HttpDelete("files/{id}")]
        public async Task<IActionResult> DeleteStudyFile(Guid id)
        {
            var userId = GetCurrentUserId();
            var result = await _selfLearnService.DeleteStudyFileAsync(userId, id);
            if (!result) return NotFound();
            return NoContent();
        }

        #endregion

        #region Statistics

        [HttpGet("stats")]
        public async Task<IActionResult> GetStudyStats([FromQuery] int days = 7)
        {
            var userId = GetCurrentUserId();
            var stats = await _selfLearnService.GetStudyStatsAsync(userId, days);
            return Ok(stats);
        }

        #endregion
    }
}
