using backend_assignment_and_management_project.Application.DTOs;
using backend_assignment_and_management_project.Application.Interfaces;
using backend_assignment_and_management_project.Domain.Entities;
using backend_assignment_and_management_project.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace backend_assignment_and_management_project.Infrastructure.Services
{
    public class TodoService : ITodoService
    {
        private readonly ApplicationDbContext _context;

        public TodoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TodoTaskDto>> GetTodosByUserAsync(Guid userId, DateTime? date = null)
        {
            var query = _context.TodoTasks
                .Where(t => t.UserId == userId);

            if (date.HasValue)
            {
                // Sử dụng khoảng thời gian (00:00:00 đến 23:59:59) để lọc chính xác theo ngày
                var startDate = DateTime.SpecifyKind(date.Value.Date, DateTimeKind.Utc);
                var endDate = startDate.AddDays(1);
                query = query.Where(t => t.StartTime >= startDate && t.StartTime < endDate);
            }

            return await query
                .OrderBy(t => t.StartTime)
                .Select(t => new TodoTaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    IsCompleted = t.IsCompleted,
                    StartTime = t.StartTime,
                    EndTime = t.EndTime,
                    UserId = t.UserId
                })
                .ToListAsync();
        }

        public async Task<TodoTaskDto?> GetTodoByIdAsync(Guid userId, Guid todoId)
        {
            return await _context.TodoTasks
                .Where(t => t.UserId == userId && t.Id == todoId)
                .Select(t => new TodoTaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    IsCompleted = t.IsCompleted,
                    StartTime = t.StartTime,
                    EndTime = t.EndTime,
                    UserId = t.UserId
                })
                .FirstOrDefaultAsync();
        }

        public async Task<TodoTaskDto> AddTodoAsync(Guid userId, CreateTodoTaskDto dto)
        {
            var todo = new TodoTask
            {
                Title = dto.Title,
                StartTime = dto.StartTime.ToUniversalTime(),
                EndTime = dto.EndTime?.ToUniversalTime(),
                IsCompleted = false,
                UserId = userId
            };

            _context.TodoTasks.Add(todo);
            await _context.SaveChangesAsync();

            return (await GetTodoByIdAsync(userId, todo.Id))!;
        }

        public async Task<TodoTaskDto?> UpdateTodoAsync(Guid userId, Guid todoId, UpdateTodoTaskDto dto)
        {
            var todo = await _context.TodoTasks
                .FirstOrDefaultAsync(t => t.Id == todoId && t.UserId == userId);

            if (todo == null) return null;

            if (dto.Title != null) todo.Title = dto.Title;
            if (dto.IsCompleted.HasValue) todo.IsCompleted = dto.IsCompleted.Value;
            if (dto.StartTime.HasValue) 
                todo.StartTime = dto.StartTime.Value.ToUniversalTime();
            if (dto.EndTime.HasValue) 
                todo.EndTime = dto.EndTime.Value.ToUniversalTime();

            await _context.SaveChangesAsync();
            return await GetTodoByIdAsync(userId, todo.Id);
        }

        public async Task<bool> DeleteTodoAsync(Guid userId, Guid todoId)
        {
            var todo = await _context.TodoTasks
                .FirstOrDefaultAsync(t => t.Id == todoId && t.UserId == userId);

            if (todo == null) return false;

            _context.TodoTasks.Remove(todo);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<TodoTaskDto?> ToggleTodoStatusAsync(Guid userId, Guid todoId)
        {
            var todo = await _context.TodoTasks
                .FirstOrDefaultAsync(t => t.Id == todoId && t.UserId == userId);

            if (todo == null) return null;

            todo.IsCompleted = !todo.IsCompleted;
            await _context.SaveChangesAsync();

            return await GetTodoByIdAsync(userId, todo.Id);
        }
    }
}
