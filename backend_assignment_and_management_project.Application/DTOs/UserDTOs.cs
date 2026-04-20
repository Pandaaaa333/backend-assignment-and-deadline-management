using System.ComponentModel.DataAnnotations;

namespace backend_assignment_and_deadline_management_project.Application.DTOs
{
    public class CreateUserRequest
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "User";
    }

    public class UpdateUserRequest
    {
        [MaxLength(100)]
        public string? Name { get; set; }

        [EmailAddress]
        [MaxLength(150)]
        public string? Email { get; set; }

        [MinLength(6)]
        public string? NewPassword { get; set; }

        public string? Role { get; set; }

        public string? AvatarUrl { get; set; }
    }

    public class UpdateProfileRequest
    {
        [MaxLength(100)]
        public string? Name { get; set; }

        [MinLength(6)]
        public string? NewPassword { get; set; }
    }

    public class ChangeRoleRequest
    {
        [Required]
        public string RoleName { get; set; } = string.Empty;
    }
}
