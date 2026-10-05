using fixBookmanagement.Data;
using fixBookmanagement.Middlewares;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Books/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Middleware phải đặt trước MapControllerRoute để request đi qua middleware trước Controller.
app.UseMiddleware<RequestLoggingMiddleware>();

// Route tương thích với đúng ví dụ trong đề bài: /Book/Detail/1
app.MapControllerRoute(
    name: "book-detail-alias",
    pattern: "Book/Detail/{id?}",
    defaults: new { controller = "Books", action = "Details" });

// Route tương thích với /Book, /Book/Create...
app.MapControllerRoute(
    name: "book-alias",
    pattern: "Book/{action=Index}/{id?}",
    defaults: new { controller = "Books" });

// Route cũ của project vẫn giữ nguyên: /Books, /Books/Details/1...
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Books}/{action=Index}/{id?}");

app.Run();
