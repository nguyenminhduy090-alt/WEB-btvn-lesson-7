using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace fixBookmanagement.Models;

public class Book
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Tên sách không được để trống")]
    [Display(Name = "Tên sách")]
    public string Title { get; set; } = "";

    [Required(ErrorMessage = "Tác giả không được để trống")]
    [Display(Name = "Tác giả")]
    public string Author { get; set; } = "";

    [Required(ErrorMessage = "Thể loại không được để trống")]
    [Display(Name = "Thể loại")]
    public string Category { get; set; } = "";

    [Range(typeof(decimal), "0.01", "100000000", ErrorMessage = "Giá phải lớn hơn 0")]
    [Display(Name = "Giá")]
    public decimal Price { get; set; }

    [Range(1900, 2100, ErrorMessage = "Năm xuất bản không hợp lệ")]
    [Display(Name = "Năm xuất bản")]
    public int PublishedYear { get; set; }

    // Đường dẫn ảnh được lưu trong database, ví dụ: /uploads/books/abc.jpg
    public string? ImagePath { get; set; }

    // ImageFile chỉ dùng để nhận file từ form upload.
    // [NotMapped] nghĩa là EF Core KHÔNG tạo cột ImageFile trong database.
    [NotMapped]
    [Display(Name = "Hình ảnh")]
    public IFormFile? ImageFile { get; set; }
}
