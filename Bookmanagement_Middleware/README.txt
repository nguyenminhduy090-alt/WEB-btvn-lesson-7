CACH CHAY:
1. Mo Bookmanagement.csproj bang Visual Studio 2022.
2. Mo database.sql trong SQL Server Management Studio va Execute.
3. Neu SQL Server cua may ban khong phai .\SQLEXPRESS, sua Server trong appsettings.json.
4. Build > Rebuild Solution.
5. Bam Run fixBookmanagement.

Project: ASP.NET Core MVC + Entity Framework Core + SQL Server, co CRUD sach.

PHAN MIDDLEWARE DA BO SUNG:
- File: Middlewares/RequestLoggingMiddleware.cs
- Ghi log thoi gian, Method va Path.
- Ghi Status Code sau khi xu ly request.
- Ghi thoi gian xu ly theo ms.
- Chan Book ID <= 0 va tra HTTP 400.
- Ho tro ca URL /Book/Detail/{id} theo de bai va /Books/Details/{id} cua project cu.
- Middleware da duoc dang ky trong Program.cs truoc MapControllerRoute.

URL TEST NHANH:
- /Book
- /Book/Detail/1
- /Book/Create
- /Book/Detail/0
- /Book/Detail/-1

Cau tra loi bao cao nam trong file BAO_CAO_MIDDLEWARE.md.
