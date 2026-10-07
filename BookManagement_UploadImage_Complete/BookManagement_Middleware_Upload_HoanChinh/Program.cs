using fixBookmanagement.Data;
using fixBookmanagement.Middlewares;
using fixBookmanagement.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Dùng SQL Server LocalDB - thường có sẵn khi cài Visual Studio với ASP.NET workload.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Books/Index");
    app.UseHsts();
}

app.UseHttpsRedirection();

// Cho phép trình duyệt đọc ảnh trong wwwroot/uploads/books.
app.UseStaticFiles();

app.UseRouting();

// ==================== MIDDLEWARE CỦA BÀI TẬP ====================
// Phải đặt TRƯỚC MapControllerRoute.
// Nhờ vậy request sẽ đi qua RequestLoggingMiddleware trước khi tới Controller.
app.UseMiddleware<RequestLoggingMiddleware>();

// Route đúng theo ví dụ trong đề: /Book/Detail/1
app.MapControllerRoute(
    name: "book-detail-alias",
    pattern: "Book/Detail/{id?}",
    defaults: new { controller = "Books", action = "Details" });

// Hỗ trợ /Book, /Book/Create...
app.MapControllerRoute(
    name: "book-alias",
    pattern: "Book/{action=Index}/{id?}",
    defaults: new { controller = "Books" });

// Route mặc định của project: /Books, /Books/Details/1...
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Books}/{action=Index}/{id?}");

// Tự tạo database và dữ liệu mẫu khi chạy lần đầu.
// Nhờ vậy không cần mở SQLQuery rồi Execute thủ công.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated();

    if (!db.Books.Any())
    {
        db.Books.AddRange(
            new Book { Title = "Nhà Giả Kim", Author = "Paulo Coelho", Category = "Tiểu thuyết", Price = 79000, PublishedYear = 1988 },
            new Book { Title = "Clean Code", Author = "Robert C. Martin", Category = "Lập trình", Price = 210000, PublishedYear = 2008 },
            new Book { Title = "Đắc Nhân Tâm", Author = "Dale Carnegie", Category = "Phát triển bản thân", Price = 89000, PublishedYear = 1936 }
        );

        db.SaveChanges();
    }
}

app.Run();
