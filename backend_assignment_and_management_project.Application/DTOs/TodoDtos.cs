using System;

namespace backend_assignment_and_management_project.Application.DTOs
{
    public class TodoTaskDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public Guid UserId { get; set; }
    }

    public class CreateTodoTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }

    public class UpdateTodoTaskDto
    {
        public string? Title { get; set; }
        public bool? IsCompleted { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
