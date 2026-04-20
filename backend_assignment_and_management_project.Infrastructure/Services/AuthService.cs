using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using backend_assignment_and_deadline_management_project.Application.DTOs;
using backend_assignment_and_deadline_management_project.Application.Interfaces;
using backend_assignment_and_deadline_management_project.Domain.Entities;
using backend_assignment_and_deadline_management_project.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using BC = BCrypt.Net.BCrypt;

namespace backend_assignment_and_deadline_management_project.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            // Kiểm tra email đã tồn tại chưa
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                throw new Exception("Email already exists.");
            }

            // Lấy role mặc định (ví dụ: 'User')
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "User");
            if (role == null)
            {
                role = new Role { Name = "User" };
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

            var token = GenerateJwtToken(user, role.Name);

            return new AuthResponse
            {
                Token = token,
                User = new UserResponse
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = role.Name
                }
            };
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || !BC.Verify(request.Password, user.Password))
            {
                throw new Exception("Invalid email or password.");
            }

            var token = GenerateJwtToken(user, user.Role.Name);

            return new AuthResponse
            {
                Token = token,
                User = new UserResponse
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    AvatarUrl = user.AvatarUrl,
                    Role = user.Role.Name
                }
            };
        }

        public async Task<UserResponse> GetCurrentUserAsync(Guid userId)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            return new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                AvatarUrl = user.AvatarUrl,
                Role = user.Role.Name
            };
        }

        private string GenerateJwtToken(User user, string role)
        {
            var jwtSettings = _configuration.GetSection("JWT");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key is missing"));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, role)
                }),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["DurationInMinutes"] ?? "1440")),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
