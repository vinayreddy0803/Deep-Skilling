/*
  Exercise 1 - Window Functions
  Demonstrates ROW_NUMBER(), RANK(), DENSE_RANK(), and OVER() with PARTITION BY
  SQL Server script - run in SSMS or sqlcmd against a test database

  Note: Prices shown are in Indian Rupees (INR) for consistency with repo convention.
*/

-- Drop table if exists (safe to rerun)
IF OBJECT_ID('dbo.Products', 'U') IS NOT NULL
    DROP TABLE dbo.Products;

CREATE TABLE dbo.Products (
    ProductID INT PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Category NVARCHAR(50) NOT NULL,
    Price DECIMAL(18,2) NOT NULL,
    Sales INT NOT NULL -- units sold
);

-- Sample data (Indian prices)
INSERT INTO dbo.Products (ProductID, Name, Category, Price, Sales) VALUES
(101, 'Wireless Mouse', 'Electronics', 899.00, 150),
(102, 'Gaming Keyboard', 'Electronics', 2499.00, 120),
(103, 'Bluetooth Speaker', 'Electronics', 1999.00, 200),
(104, 'Smartwatch', 'Electronics', 4999.00, 180),
(201, 'Men T-Shirt', 'Apparel', 599.00, 300),
(202, 'Jeans', 'Apparel', 1299.00, 220),
(203, 'Jacket', 'Apparel', 1999.00, 80),
(301, 'Cookware Set', 'Home & Kitchen', 3499.00, 95),
(302, 'Vacuum Cleaner', 'Home & Kitchen', 7499.00, 60),
(303, 'LED Lamp', 'Home & Kitchen', 799.00, 210);

-- 1) ROW_NUMBER() example: global ranking by Sales
SELECT
    ProductID,
    Name,
    Category,
    Sales,
    ROW_NUMBER() OVER (ORDER BY Sales DESC) AS RowNum
FROM dbo.Products
ORDER BY RowNum;

-- 2) RANK() vs DENSE_RANK(): show ties effect
-- Add deliberate tie on Sales
INSERT INTO dbo.Products (ProductID, Name, Category, Price, Sales) VALUES (1041, 'Budget Headphones', 'Electronics', 799.00, 180);

SELECT
    ProductID,
    Name,
    Category,
    Sales,
    RANK() OVER (ORDER BY Sales DESC) AS RankBySales,
    DENSE_RANK() OVER (ORDER BY Sales DESC) AS DenseRankBySales
FROM dbo.Products
ORDER BY Sales DESC;

-- 3) PARTITION BY example: ranking within each Category
SELECT
    ProductID,
    Name,
    Category,
    Sales,
    ROW_NUMBER() OVER (PARTITION BY Category ORDER BY Sales DESC) AS RowInCategory,
    RANK() OVER (PARTITION BY Category ORDER BY Sales DESC) AS RankInCategory,
    DENSE_RANK() OVER (PARTITION BY Category ORDER BY Sales DESC) AS DenseRankInCategory
FROM dbo.Products
ORDER BY Category, Sales DESC;

-- 4) Using window aggregates: moving average (last 3 products by Sales within category)
SELECT
    ProductID,
    Name,
    Category,
    Sales,
    AVG(CAST(Sales AS FLOAT)) OVER (PARTITION BY Category ORDER BY Sales DESC ROWS BETWEEN 2 PRECEDING AND CURRENT ROW) AS MovingAvg_Last3
FROM dbo.Products
ORDER BY Category, Sales DESC;

-- Cleanup comment: remove the extra tie row if re-running tests
-- DELETE FROM dbo.Products WHERE ProductID = 1041;
