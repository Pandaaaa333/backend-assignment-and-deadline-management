using Microsoft.EntityFrameworkCore;
using backend_assignment_and_deadline_management_project.Domain.Entities;

namespace backend_assignment_and_deadline_management_project.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // Đăng ký các thực thể (Bảng) theo đúng sơ đồ của bạn
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        // Các bảng khác (Posts, Comments...) bạn sẽ thêm tương tự sau này

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình để EF Core tự động chuyển tên bảng và cột sang snake_case như trong ảnh ERD
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                // Ví dụ: Đổi tên bảng từ "Users" thành "users"
                entity.SetTableName(entity.GetTableName()?.ToLower());

                // Ví dụ: Đổi tên cột từ "AvatarUrl" thành "avatar_url"
                foreach (var property in entity.GetProperties())
                {
                    property.SetColumnName(System.Text.RegularExpressions.Regex.Replace(
                        property.Name, "([a-z])([A-Z])", "$1_$2").ToLower());
                }
            }
        }
    }
}