using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace fixBookmanagement.Middlewares;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        var method = context.Request.Method;
        var path = context.Request.Path.ToString();

        Console.WriteLine($"[{time}] Method: {method} - Path: {path}");

        // Project hiện tại dùng /Books/Details/{id}.
        // Đồng thời hỗ trợ dạng URL trong đề bài: /Book/Detail/{id}.
        if (IsInvalidBookDetailPath(path))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "text/plain; charset=utf-8";
            await context.Response.WriteAsync("Book id không hợp lệ");

            stopwatch.Stop();
            Console.WriteLine($"Status Code: {context.Response.StatusCode} - Time: {stopwatch.ElapsedMilliseconds} ms");
            return;
        }

        await _next(context);

        stopwatch.Stop();
        Console.WriteLine($"Status Code: {context.Response.StatusCode} - Time: {stopwatch.ElapsedMilliseconds} ms");
    }

    private static bool IsInvalidBookDetailPath(string path)
    {
        var parts = path.Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 3)
        {
            return false;
        }

        var isBookDetailUrl =
            (parts[0].Equals("Book", StringComparison.OrdinalIgnoreCase) &&
             parts[1].Equals("Detail", StringComparison.OrdinalIgnoreCase)) ||
            (parts[0].Equals("Books", StringComparison.OrdinalIgnoreCase) &&
             parts[1].Equals("Details", StringComparison.OrdinalIgnoreCase));

        return isBookDetailUrl && int.TryParse(parts[2], out var id) && id <= 0;
    }
}
