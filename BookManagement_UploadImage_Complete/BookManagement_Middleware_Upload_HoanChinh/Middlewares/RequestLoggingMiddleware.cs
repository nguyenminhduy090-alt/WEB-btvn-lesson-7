using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace fixBookmanagement.Middlewares;

/// <summary>
/// Middleware này xử lý request TRƯỚC và SAU Controller.
/// Đây là phần quan trọng nhất của bài tập Middleware.
/// </summary>
public class RequestLoggingMiddleware
{
    // _next đại diện cho bước tiếp theo trong pipeline.
    // Bước tiếp theo có thể là middleware khác hoặc Controller.
    private readonly RequestDelegate _next;

    public RequestLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // ==================== 1. XỬ LÝ TRƯỚC CONTROLLER ====================
        // Lấy thời gian hiện tại, Method và Path theo đúng yêu cầu đề bài.
        // .fff giúp hiển thị thêm milliseconds.
        var time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        var method = context.Request.Method;
        var path = context.Request.Path.ToString();

        Console.WriteLine($"[{time}] Method: {method} - Path: {path}");

        var stopwatch = Stopwatch.StartNew();

        // ==================== 2. CHẶN URL KHÔNG HỢP LỆ ====================
        // Nếu URL là /Book/Detail/0 hoặc /Book/Detail/-1 (nói chung id <= 0)
        // thì Middleware tự trả về HTTP 400 và KHÔNG cho request vào Controller.
        if (IsInvalidBookDetailPath(path))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "text/plain; charset=utf-8";
            await context.Response.WriteAsync("Book id không hợp lệ");

            stopwatch.Stop();
            Console.WriteLine($"Status Code: {context.Response.StatusCode} - Time: {stopwatch.ElapsedMilliseconds} ms");

            // return kết thúc InvokeAsync tại đây.
            // Vì không gọi await _next(context), request sẽ không đi tiếp tới Controller.
            return;
        }

        // ==================== 3. CHO REQUEST ĐI TIẾP ====================
        // Dòng này chuyển request sang middleware tiếp theo hoặc Controller.
        // Sau khi Controller xử lý xong, chương trình quay lại dòng phía dưới.
        await _next(context);

        // ==================== 4. XỬ LÝ SAU CONTROLLER ====================
        // Lúc này đã có mã trạng thái cuối cùng: 200, 404, 403...
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

        // Hỗ trợ cả URL đúng theo đề /Book/Detail/{id}
        // và URL gốc của project /Books/Details/{id}.
        var isBookDetailUrl =
            (parts[0].Equals("Book", StringComparison.OrdinalIgnoreCase) &&
             parts[1].Equals("Detail", StringComparison.OrdinalIgnoreCase)) ||
            (parts[0].Equals("Books", StringComparison.OrdinalIgnoreCase) &&
             parts[1].Equals("Details", StringComparison.OrdinalIgnoreCase));

        return isBookDetailUrl &&
               int.TryParse(parts[2], out var id) &&
               id <= 0;
    }
}
