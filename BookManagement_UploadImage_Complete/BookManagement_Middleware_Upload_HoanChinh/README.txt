BOOK MANAGEMENT - MIDDLEWARE + UPLOAD HINH ANH
================================================

1. CACH MO VA CHAY TREN VISUAL STUDIO
--------------------------------------
- Can Visual Studio 2022 co workload: ASP.NET and web development.
- Mo file: fixBookmanagement.csproj
  (hoac mo fixBookmanagement.slnx neu Visual Studio cua ban ho tro .slnx)
- Cho Visual Studio Restore NuGet packages neu duoc hoi.
- Bam Ctrl + F5 hoac nut Run.

Project mac dinh dung SQL Server LocalDB:
Server=(localdb)\MSSQLLocalDB
Database=BookManagementMiddlewareUploadDB

Program.cs se tu tao database va them 3 sach mau khi chay lan dau.
KHONG can chay database.sql thu cong.

2. CAC URL TEST MIDDLEWARE THEO DE
----------------------------------
/Book
/Book/Detail/1
/Book/Create
/Book/Detail/0
/Book/Detail/-1

Mo cua so Console/Output khi chay de xem log, vi du:
[2026-10-07 08:30:15.123] Method: GET - Path: /Book
Status Code: 200 - Time: 35 ms

/Book/Detail/0 va /Book/Detail/-1 se tra:
Book id không hợp lệ
HTTP Status Code: 400

3. PHAN MIDDLEWARE QUAN TRONG
-----------------------------
File: Middlewares/RequestLoggingMiddleware.cs

- Truoc await _next(context):
  Middleware doc Method, Path, thoi gian va co the chan request.

- await _next(context):
  Chuyen request sang buoc tiep theo, cuoi cung la Controller.

- Sau await _next(context):
  Controller da xu ly xong, nen middleware doc duoc Status Code.

- Neu middleware return truoc await _next(context):
  Request dung tai middleware va Controller khong duoc goi.

4. UPLOAD HINH ANH
------------------
- Vao /Book/Create.
- Dien thong tin sach.
- Chon anh JPG/JPEG/PNG/WEBP, toi da 5 MB.
- Anh duoc luu vao: wwwroot/uploads/books
- Duong dan anh duoc luu trong cot ImagePath.
- Index va Details se hien thi anh.
- Edit co the thay anh moi.
- Delete se xoa ca file anh neu anh ton tai.

5. CAC FILE NEN DOC DE HIEU BAI
-------------------------------
- Middlewares/RequestLoggingMiddleware.cs  <-- chu thich Middleware rat ro
- Program.cs                               <-- vi tri dang ky Middleware
- Controllers/BooksController.cs           <-- upload anh
- Models/Book.cs                            <-- ImagePath va ImageFile
- Views/Books/Create.cshtml                 <-- form multipart/form-data

6. LUU Y
--------
Khong co JavaScript logging. Console chi in cac log quan trong dung theo de Middleware:
- Thoi gian + Method + Path
- Status Code
- Thoi gian xu ly (ms)
