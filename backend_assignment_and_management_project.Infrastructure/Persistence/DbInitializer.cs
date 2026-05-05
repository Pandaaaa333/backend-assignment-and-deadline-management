using backend_assignment_and_management_project.Domain.Entities;
using backend_assignment_and_management_project.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace backend_assignment_and_management_project.Infrastructure.Persistence
{
    public static class DbInitializer
    {
        public static async Task Seed(ApplicationDbContext context)
        {

            var subjects = new List<Subject>
            {
                new Subject { Name = "Cấu trúc dữ liệu", Description = "Học về các cấu trúc lưu trữ dữ liệu hiệu quả." },
                new Subject { Name = "Giải thuật", Description = "Các phương pháp giải quyết vấn đề bằng máy tính." },
                new Subject { Name = "Trí tuệ nhân tạo", Description = "Khám phá thế giới của AI và Machine Learning." },
                new Subject { Name = "Phát triển Mobile", Description = "Xây dựng ứng dụng với Flutter và React Native." },
                new Subject { Name = "Lập trình Web", Description = "Xây dựng website hiện đại với React, Node.js." },
                new Subject { Name = "Cơ sở dữ liệu", Description = "Quản lý dữ liệu với SQL và NoSQL." },
                new Subject { Name = "Mạng máy tính", Description = "Tìm hiểu về giao thức và hạ tầng mạng." },
                new Subject { Name = "Hệ điều hành", Description = "Nguyên lý hoạt động của Windows, Linux, macOS." },
                new Subject { Name = "An toàn thông tin", Description = "Bảo mật dữ liệu và phòng chống tấn công mạng." },
                new Subject { Name = "Kỹ thuật phần mềm", Description = "Quy trình xây dựng phần mềm chuyên nghiệp." },
                new Subject { Name = "Đồ họa máy tính", Description = "Xử lý hình ảnh và không gian 3D." },
                new Subject { Name = "Điện toán đám mây", Description = "Triển khai dịch vụ trên AWS, Azure, Google Cloud." },
                new Subject { Name = "Blockchain", Description = "Công nghệ chuỗi khối và hợp đồng thông minh." },
                new Subject { Name = "Internet of Things", Description = "Kết nối vạn vật qua internet." },
                new Subject { Name = "Xác suất thống kê", Description = "Nền tảng toán học cho phân tích dữ liệu." },
                new Subject { Name = "Kinh tế học", Description = "Các nguyên lý kinh tế cơ bản." },
                new Subject { Name = "Quản trị kinh doanh", Description = "Kỹ năng quản lý và vận hành doanh nghiệp." },
                new Subject { Name = "Tiếng Anh chuyên ngành", Description = "Nâng cao vốn từ vựng kỹ thuật." },
                new Subject { Name = "Kỹ năng mềm", Description = "Giao tiếp, làm việc nhóm và thuyết trình." },
                new Subject { Name = "Tâm lý học", Description = "Tìm hiểu về hành vi và tư duy con người." },
                new Subject { Name = "Phát triển Game", Description = "Lập trình game với Unity, Unreal Engine." },
                new Subject { Name = "Khoa học dữ liệu", Description = "Phân tích và khai phá dữ liệu quy mô lớn." },
                new Subject { Name = "Thiết kế UI/UX", Description = "Thiết kế trải nghiệm và giao diện người dùng." },
                new Subject { Name = "Kỹ thuật đồ họa", Description = "Xử lý hình ảnh và không gian 3D nâng cao." },
                new Subject { Name = "Hệ thống nhúng", Description = "Lập trình vi điều khiển và thiết bị thông minh." }
            };

            foreach (var subject in subjects)
            {
                if (!await context.Subjects.AnyAsync(s => s.Name == subject.Name))
                {
                    await context.Subjects.AddAsync(subject);
                }
            }

            await context.SaveChangesAsync();
        }

        public static async Task SeedPosts(ApplicationDbContext context, User adminUser)
        {
            if (await context.Posts.AnyAsync()) return;

            var subjects = await context.Subjects.ToListAsync();
            var posts = new List<Post>();

            foreach (var subject in subjects)
            {
                posts.Add(new Post
                {
                    Content = $"Tài liệu tổng hợp môn {subject.Name}. Mọi người cùng tham khảo nhé!",
                    UserId = adminUser.Id,
                    SubjectId = subject.Id,
                    CreatedAt = DateTime.UtcNow.AddHours(-24)
                });

                posts.Add(new Post
                {
                    Content = $"Thông báo quan trọng về lịch thi và kiểm tra giữa kỳ môn {subject.Name}.",
                    UserId = adminUser.Id,
                    SubjectId = subject.Id,
                    CreatedAt = DateTime.UtcNow.AddHours(-5)
                });

                posts.Add(new Post
                {
                    Content = $"Thảo luận về các chủ đề nâng cao trong {subject.Name}. Ai có thắc mắc gì thì comment bên dưới nhé!",
                    UserId = adminUser.Id,
                    SubjectId = subject.Id,
                    CreatedAt = DateTime.UtcNow.AddMinutes(-30)
                });
            }

            await context.Posts.AddRangeAsync(posts);
            await context.SaveChangesAsync();
        }
    }
}
