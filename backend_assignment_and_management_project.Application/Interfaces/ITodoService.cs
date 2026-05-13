using backend_assignment_and_management_project.Application.DTOs;

namespace backend_assignment_and_management_project.Application.Interfaces
{
    public interface ITodoService
    {
        Task<IEnumerable<TodoTaskDto>> GetTodosByUserAsync(Guid userId, DateTime? date = null);
        Task<TodoTaskDto?> GetTodoByIdAsync(Guid userId, Guid todoId);
        Task<TodoTaskDto> AddTodoAsync(Guid userId, CreateTodoTaskDto dto);
        Task<TodoTaskDto?> UpdateTodoAsync(Guid userId, Guid todoId, UpdateTodoTaskDto dto);
        Task<bool> DeleteTodoAsync(Guid userId, Guid todoId);
        Task<TodoTaskDto?> ToggleTodoStatusAsync(Guid userId, Guid todoId);
    }
}
