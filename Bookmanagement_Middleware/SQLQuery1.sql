IF DB_ID('BookStoreFriendDB') IS NULL
    CREATE DATABASE BookStoreFriendDB;
GO
USE BookStoreFriendDB;
GO
IF OBJECT_ID('Books','U') IS NULL
BEGIN
    CREATE TABLE Books(
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Title NVARCHAR(200) NOT NULL,
        Author NVARCHAR(150) NOT NULL,
        Category NVARCHAR(100) NOT NULL,
        Price DECIMAL(18,2) NOT NULL,
        PublishedYear INT NOT NULL
    );
END
GO
IF NOT EXISTS (SELECT 1 FROM Books)
BEGIN
    INSERT INTO Books(Title,Author,Category,Price,PublishedYear) VALUES
    (N'Kỷ Luật Tự Giác',N'Nguyễn Hiến Lê',N'Kỹ năng',99000,2022),
    (N'Nhà Giả Kim',N'Paulo Coelho',N'Tiểu thuyết',79000,1988),
    (N'Clean Code',N'Robert C. Martin',N'Lập trình',210000,2008),
    (N'Đắc Nhân Tâm',N'Dale Carnegie',N'Phát triển bản thân',89000,1936),
    (N'Tư Duy Nhanh Và Chậm',N'Daniel Kahneman',N'Tâm lý',145000,2011);
END
GO
