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
    public class TodosController : ControllerBase
    {
        private readonly ITodoService _todoService;

        public TodosController(ITodoService todoService)
        {
            _todoService = todoService;
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) throw new UnauthorizedAccessException();
            return Guid.Parse(userIdClaim.Value);
        }

        [HttpGet]
        public async Task<IActionResult> GetTodos([FromQuery] DateTime? date = null)
        {
            var userId = GetCurrentUserId();
            var todos = await _todoService.GetTodosByUserAsync(userId, date);
            return Ok(todos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTodoById(Guid id)
        {
            var userId = GetCurrentUserId();
            var todo = await _todoService.GetTodoByIdAsync(userId, id);
            if (todo == null) return NotFound();
            return Ok(todo);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTodo([FromBody] CreateTodoTaskDto dto)
        {
            var userId = GetCurrentUserId();
            var todo = await _todoService.AddTodoAsync(userId, dto);
            return CreatedAtAction(nameof(GetTodoById), new { id = todo.Id }, todo);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodo(Guid id, [FromBody] UpdateTodoTaskDto dto)
        {
            var userId = GetCurrentUserId();
            var todo = await _todoService.UpdateTodoAsync(userId, id, dto);
            if (todo == null) return NotFound();
            return Ok(todo);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodo(Guid id)
        {
            var userId = GetCurrentUserId();
            var result = await _todoService.DeleteTodoAsync(userId, id);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpPatch("{id}/toggle")]
        public async Task<IActionResult> ToggleTodoStatus(Guid id)
        {
            var userId = GetCurrentUserId();
            var todo = await _todoService.ToggleTodoStatusAsync(userId, id);
            if (todo == null) return NotFound();
            return Ok(todo);
        }
    }
}
