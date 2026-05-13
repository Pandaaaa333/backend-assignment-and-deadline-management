using backend_assignment_and_management_project.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace backend_assignment_and_management_project.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IStorageService _storageService;

        public FilesController(IStorageService storageService)
        {
            _storageService = storageService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file, [FromQuery] string folder = "uploads")
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            try
            {
                var filePath = await _storageService.UploadFileAsync(file.OpenReadStream(), file.FileName, file.ContentType, folder);
                var presignedUrl = await _storageService.GetPresignedUrlAsync(filePath);
                
                return Ok(new { 
                    filePath = filePath,
                    url = presignedUrl 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("presigned-url")]
        public async Task<IActionResult> GetPresignedUrl([FromQuery] string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return BadRequest("File path is required.");

            try
            {
                var url = await _storageService.GetPresignedUrlAsync(filePath);
                return Ok(new { url = url });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return BadRequest("File path is required.");

            try
            {
                await _storageService.DeleteFileAsync(filePath);
                return Ok("File deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
