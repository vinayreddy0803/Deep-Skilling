/*
  Exercise 4 - Stored Procedures
  SQL Server script demonstrating creating stored procedures, executing them,
  and returning values using OUTPUT parameters.

  Prices and amounts are in Indian Rupees (INR).
*/

-- Drop existing table and procs for idempotence
IF OBJECT_ID('dbo.Products', 'U') IS NOT NULL
    DROP TABLE dbo.Products;
IF OBJECT_ID('dbo.usp_GetProductSales', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_GetProductSales;
IF OBJECT_ID('dbo.usp_UpdateProductPrice', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_UpdateProductPrice;
IF OBJECT_ID('dbo.usp_GetCategorySales', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_GetCategorySales;

CREATE TABLE dbo.Products (
    ProductID INT PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Category NVARCHAR(50) NOT NULL,
    Price DECIMAL(18,2) NOT NULL,
    Sales INT NOT NULL
);

-- Sample data (INR)
INSERT INTO dbo.Products (ProductID, Name, Category, Price, Sales) VALUES
(101, 'Wireless Mouse', 'Electronics', 899.00, 150),
(102, 'Gaming Keyboard', 'Electronics', 2499.00, 120),
(103, 'Bluetooth Speaker', 'Electronics', 1999.00, 200),
(104, 'Smartwatch', 'Electronics', 4999.00, 180),
(201, 'Men T-Shirt', 'Apparel', 599.00, 300),
(202, 'Jeans', 'Apparel', 1299.00, 220),
(203, 'Jacket', 'Apparel', 1999.00, 80);

-- 1) Stored procedure to get product sales via OUTPUT parameter
CREATE PROCEDURE dbo.usp_GetProductSales
    @ProductID INT,
    @Sales INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT @Sales = Sales FROM dbo.Products WHERE ProductID = @ProductID;
    IF @Sales IS NULL SET @Sales = 0;
END;

-- 2) Stored procedure to update product price and return rows affected via OUTPUT
CREATE PROCEDURE dbo.usp_UpdateProductPrice
    @ProductID INT,
    @NewPrice DECIMAL(18,2),
    @RowsAffected INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Products SET Price = @NewPrice WHERE ProductID = @ProductID;
    SET @RowsAffected = @@ROWCOUNT;
END;

-- 3) Stored procedure to compute total sales for a category via OUTPUT
CREATE PROCEDURE dbo.usp_GetCategorySales
    @Category NVARCHAR(50),
    @TotalSales INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT @TotalSales = SUM(Sales) FROM dbo.Products WHERE Category = @Category;
    IF @TotalSales IS NULL SET @TotalSales = 0;
END;

/*
  Example usage and expected behavior (run these lines in SSMS):

  DECLARE @s INT;
  EXEC dbo.usp_GetProductSales @ProductID = 101, @Sales = @s OUTPUT;
  SELECT @s AS Sales; -- expected 150

  DECLARE @rows INT;
  EXEC dbo.usp_UpdateProductPrice @ProductID = 102, @NewPrice = 2599.00, @RowsAffected = @rows OUTPUT;
  SELECT @rows AS RowsAffected; -- expected 1

  DECLARE @total INT;
  EXEC dbo.usp_GetCategorySales @Category = 'Electronics', @TotalSales = @total OUTPUT;
  SELECT @total AS ElectronicsTotalSales; -- expected 650

*/
