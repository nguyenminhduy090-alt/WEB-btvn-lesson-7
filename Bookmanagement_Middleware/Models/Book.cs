using System.ComponentModel.DataAnnotations;

namespace fixBookmanagement.Models;

public class Book
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Nhập tên sách")]
    [Display(Name = "Tên sách")]
    public string Title { get; set; } = "";

    [Required(ErrorMessage = "Nhập tác giả")]
    [Display(Name = "Tác giả")]
    public string Author { get; set; } = "";

    [Required(ErrorMessage = "Nhập thể loại")]
    [Display(Name = "Thể loại")]
    public string Category { get; set; } = "";

    [Range(typeof(decimal), "0.01", "100000000", ErrorMessage = "Giá phải lớn hơn 0")]
    [Display(Name = "Giá")]
    public decimal Price { get; set; }

    [Range(1900, 2100, ErrorMessage = "Năm không hợp lệ")]
    [Display(Name = "Năm xuất bản")]
    public int PublishedYear { get; set; }
}
