using backend_assignment_and_deadline_management_project.Application.DTOs;

namespace backend_assignment_and_deadline_management_project.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<UserResponse> GetCurrentUserAsync(Guid userId);
    }
}
