using fixBookmanagement.Data;
using fixBookmanagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace fixBookmanagement.Controllers;

public class BooksController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
    private const long MaxImageSize = 5 * 1024 * 1024; // 5 MB

    public BooksController(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    public async Task<IActionResult> Index(string? keyword)
    {
        var query = _context.Books.AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(x =>
                x.Title.Contains(keyword) ||
                x.Author.Contains(keyword) ||
                x.Category.Contains(keyword));
        }

        ViewBag.Keyword = keyword;
        return View(await query.OrderBy(x => x.Id).ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var book = await _context.Books.FindAsync(id);
        if (book == null) return NotFound();

        return View(book);
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Book book)
    {
        // Kiểm tra file ảnh trước khi lưu sách.
        if (book.ImageFile != null)
        {
            ValidateImage(book.ImageFile);
        }

        if (!ModelState.IsValid)
        {
            return View(book);
        }

        // Nếu người dùng chọn ảnh, lưu ảnh vào wwwroot/uploads/books.
        if (book.ImageFile != null)
        {
            book.ImagePath = await SaveImageAsync(book.ImageFile);
        }

        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var book = await _context.Books.FindAsync(id);
        if (book == null) return NotFound();

        return View(book);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Book book)
    {
        if (id != book.Id) return NotFound();

        var existingBook = await _context.Books.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (existingBook == null) return NotFound();

        if (book.ImageFile != null)
        {
            ValidateImage(book.ImageFile);
        }

        if (!ModelState.IsValid)
        {
            // Nếu validation lỗi thì vẫn giữ ảnh cũ để View hiển thị.
            book.ImagePath = existingBook.ImagePath;
            return View(book);
        }

        // Mặc định giữ lại ảnh cũ.
        book.ImagePath = existingBook.ImagePath;

        // Nếu chọn ảnh mới thì lưu ảnh mới và xóa ảnh cũ.
        if (book.ImageFile != null)
        {
            var oldImagePath = existingBook.ImagePath;
            book.ImagePath = await SaveImageAsync(book.ImageFile);
            DeleteImageIfExists(oldImagePath);
        }

        _context.Books.Update(book);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var book = await _context.Books.FindAsync(id);
        if (book == null) return NotFound();

        return View(book);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var book = await _context.Books.FindAsync(id);

        if (book != null)
        {
            DeleteImageIfExists(book.ImagePath);
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    // Kiểm tra định dạng và dung lượng file upload.
    private void ValidateImage(IFormFile imageFile)
    {
        var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();

        if (!AllowedImageExtensions.Contains(extension))
        {
            ModelState.AddModelError(nameof(Book.ImageFile), "Chỉ chấp nhận ảnh JPG, JPEG, PNG hoặc WEBP.");
        }

        if (imageFile.Length <= 0)
        {
            ModelState.AddModelError(nameof(Book.ImageFile), "File ảnh không hợp lệ.");
        }
        else if (imageFile.Length > MaxImageSize)
        {
            ModelState.AddModelError(nameof(Book.ImageFile), "Ảnh không được lớn hơn 5 MB.");
        }
    }

    // Lưu ảnh với tên ngẫu nhiên để tránh trùng tên file.
    private async Task<string> SaveImageAsync(IFormFile imageFile)
    {
        var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
        var fileName = $"{Guid.NewGuid():N}{extension}";

        var webRootPath = _environment.WebRootPath
            ?? Path.Combine(_environment.ContentRootPath, "wwwroot");

        var uploadFolder = Path.Combine(webRootPath, "uploads", "books");
        Directory.CreateDirectory(uploadFolder);

        var physicalPath = Path.Combine(uploadFolder, fileName);

        await using var stream = new FileStream(physicalPath, FileMode.Create);
        await imageFile.CopyToAsync(stream);

        // Đường dẫn bắt đầu bằng / để dùng trực tiếp trong thẻ <img src="...">.
        return $"/uploads/books/{fileName}";
    }

    private void DeleteImageIfExists(string? imagePath)
    {
        if (string.IsNullOrWhiteSpace(imagePath))
        {
            return;
        }

        var webRootPath = _environment.WebRootPath
            ?? Path.Combine(_environment.ContentRootPath, "wwwroot");

        var relativePath = imagePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var physicalPath = Path.Combine(webRootPath, relativePath);

        if (System.IO.File.Exists(physicalPath))
        {
            System.IO.File.Delete(physicalPath);
        }
    }
}
