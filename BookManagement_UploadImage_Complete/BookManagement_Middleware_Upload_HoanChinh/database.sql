-- File này chỉ để tham khảo nếu muốn tự tạo database bằng SQL Server.
-- Project mặc định dùng LocalDB và Program.cs sẽ tự tạo database khi chạy lần đầu.

IF DB_ID('BookManagementMiddlewareUploadDB') IS NULL
    CREATE DATABASE BookManagementMiddlewareUploadDB;
GO

USE BookManagementMiddlewareUploadDB;
GO

IF OBJECT_ID('Books', 'U') IS NULL
BEGIN
    CREATE TABLE Books(
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Title NVARCHAR(MAX) NOT NULL,
        Author NVARCHAR(MAX) NOT NULL,
        Category NVARCHAR(MAX) NOT NULL,
        Price DECIMAL(18,2) NOT NULL,
        PublishedYear INT NOT NULL,
        ImagePath NVARCHAR(MAX) NULL
    );
END
GO
