using backend_assignment_and_deadline_management_project.Application.DTOs;

namespace backend_assignment_and_deadline_management_project.Application.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponse>> GetAllAsync();
        Task<UserResponse> GetByIdAsync(Guid id);
        Task<UserResponse> CreateAsync(CreateUserRequest request);
        Task<UserResponse> UpdateAsync(Guid id, UpdateUserRequest request);
        Task<bool> DeleteAsync(Guid id);
        Task<UserResponse> ChangeRoleAsync(Guid userId, string roleName);
        Task<UserResponse> UpdateProfileAsync(Guid userId, UpdateProfileRequest request);
    }
}
