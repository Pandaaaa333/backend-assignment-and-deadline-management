using backend_assignment_and_deadline_management_project.Application.DTOs;
using backend_assignment_and_deadline_management_project.Application.Interfaces;
using backend_assignment_and_deadline_management_project.Domain.Entities;
using backend_assignment_and_deadline_management_project.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using BC = BCrypt.Net.BCrypt;

namespace backend_assignment_and_deadline_management_project.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserResponse>> GetAllAsync()
        {
            return await _context.Users
                .Include(u => u.Role)
                .Select(u => new UserResponse
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    AvatarUrl = u.AvatarUrl,
                    Role = u.Role.Name
                })
                .ToListAsync();
        }

        public async Task<UserResponse> GetByIdAsync(Guid id)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return null!;

            return new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                AvatarUrl = user.AvatarUrl,
                Role = user.Role.Name
            };
        }

        public async Task<UserResponse> CreateAsync(CreateUserRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                throw new Exception("Email already exists.");
            }

            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == request.Role);
            if (role == null)
            {
                role = new Role { Name = request.Role };
                _context.Roles.Add(role);
                await _context.SaveChangesAsync();
            }

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                Password = BC.HashPassword(request.Password),
                RoleId = role.Id
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = role.Name
            };
        }

        public async Task<UserResponse> UpdateAsync(Guid id, UpdateUserRequest request)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) throw new Exception("User not found.");

            if (!string.IsNullOrEmpty(request.Name)) user.Name = request.Name;
            if (!string.IsNullOrEmpty(request.Email)) user.Email = request.Email;
            if (!string.IsNullOrEmpty(request.NewPassword)) user.Password = BC.HashPassword(request.NewPassword);
            if (!string.IsNullOrEmpty(request.AvatarUrl)) user.AvatarUrl = request.AvatarUrl;

            if (!string.IsNullOrEmpty(request.Role))
            {
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == request.Role);
                if (role == null)
                {
                    role = new Role { Name = request.Role };
                    _context.Roles.Add(role);
                    await _context.SaveChangesAsync();
                }
                user.RoleId = role.Id;
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                AvatarUrl = user.AvatarUrl,
                Role = user.Role.Name
            };
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<UserResponse> ChangeRoleAsync(Guid userId, string roleName)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) throw new Exception("User not found.");

            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
            if (role == null)
            {
                role = new Role { Name = roleName };
                _context.Roles.Add(role);
                await _context.SaveChangesAsync();
            }

            user.RoleId = role.Id;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                AvatarUrl = user.AvatarUrl,
                Role = role.Name
            };
        }

        public async Task<UserResponse> UpdateProfileAsync(Guid userId, UpdateProfileRequest request)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            if (!string.IsNullOrEmpty(request.Name))
            {
                user.Name = request.Name;
            }

            if (!string.IsNullOrEmpty(request.NewPassword))
            {
                user.Password = BC.HashPassword(request.NewPassword);
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                AvatarUrl = user.AvatarUrl,
                Role = user.Role.Name
            };
        }
    }
}
