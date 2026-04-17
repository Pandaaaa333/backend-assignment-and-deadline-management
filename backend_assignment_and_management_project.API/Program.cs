using backend_assignment_and_deadline_management_project.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Thêm Controller (Để sau này viết API trong thư mục Controllers)
builder.Services.AddControllers();

// 2. Cấu hình OpenAPI/Swagger
builder.Services.AddOpenApi();

// 3. ĐĂNG KÝ DBCONTEXT KẾT NỐI POSTGRESQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// 4. Cấu hình HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    // Nếu bạn muốn dùng giao diện Swagger để test cho sướng, có thể thêm app.UseSwaggerUI() ở đây
}

app.UseHttpsRedirection();

// 5. Map các Controller
app.MapControllers();

app.Run();