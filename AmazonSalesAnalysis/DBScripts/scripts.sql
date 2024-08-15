
--- Table Creation
Create TABLE Category
(
    Id int PRIMARY KEY IDENTITY(1,1),
    Category nvarchar(max)
)

Create TABLE SubCategory
(
    Id int PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(max),
    CategoryId int REFERENCES Category(Id)
)

Create Table Product
(
    Id int Primary Key IDENTITY(1,1),
    Name NVARCHAR(max),
    SubCategoryId int REFERENCES SubCategory(Id),
    ImageUrl NVARCHAR(max),
    ProductUrl NVARCHAR(max),
    Price FLOAT ,
    DiscountPrice FLOAT
)

CREATE Table Rating
(
    Id int PRIMARY Key IDENTITY(1,1),
    ProductId int REFERENCES Product(Id),
    Rating FLOAT
)


CREATE TABLE Invoices
(
    InvoiceNo INT PRIMARY KEY IDENTITY(1,1),
    ProductId INT,
    Quantity INT,
    InvoiceDate DATETIME,
    Price FLOAT,
    CustomerId INT,
    Location VARCHAR(255),
);

create table ProductAnalysis
(

    productId int,
    AverageOrderQuantity int,
    RepeatPercentage int,
    minimumPrice float,
    maximumPrice float,
    mostSoldAtLocation nvarchar(max),
    avgRating float

);

Create Table CategoryAnalysis(
    Id int, 
    HighestRatedProductId int, 
    HighestRatedProduct NVARCHAR(max),
    Average_Rating float, 
    MostSoldProductId int, 
    MostSoldProduct NVARCHAR(max), 
    Total_Sold INT
    )


Create Table SubCategoryAnalysis(
    Id int, 
    HighestRatedProductId int, 
    HighestRatedProduct NVARCHAR(max),
    Average_Rating float, 
    MostSoldProductId int, 
    MostSoldProduct NVARCHAR(max), 
    Total_Sold INT
    )

---1.  Most Highly Rated Product under each category 

SELECT Category_Id, Category, Subcategory, Product, AVG(Rating) AS Average_Rating
FROM (
    SELECT c.Id as Category_Id, c.Category, s.Name AS Subcategory, p.Name AS Product, AVG(r.Rating) AS Rating,
        ROW_NUMBER() OVER (PARTITION BY c.Category ORDER BY AVG(r.Rating) DESC) AS rank
    FROM Category c
        INNER JOIN SubCategory s ON c.Id = s.CategoryId
        INNER JOIN Product p ON s.Id = p.SubCategoryId
        INNER JOIN Rating r ON p.Id = r.ProductId
    GROUP BY c.Category, s.Name, p.Name, c.Id
) ranked
WHERE rank = 1
GROUP BY Category, Subcategory, Product,Category_Id

---2.  Most Highly Rated Prouct under each sub category 

SELECT ranked.Subcategory, ranked.SubCategoryId, ranked.ProductId, ranked.Product, AVG(ranked.Rating) AS Average_Rating
FROM (
    SELECT s.Name AS Subcategory, p.SubCategoryId, p.Id AS ProductId, p.Name AS Product, AVG(r.Rating) AS Rating,
        ROW_NUMBER() OVER (PARTITION BY s.Id ORDER BY AVG(r.Rating) DESC) AS rank
    FROM SubCategory s
        INNER JOIN Product p ON s.Id = p.SubCategoryId
        INNER JOIN Rating r ON p.Id = r.ProductId
    GROUP BY s.Id, s.Name, p.SubCategoryId, p.Id, p.Name
) ranked
WHERE ranked.rank = 1
GROUP BY ranked.Subcategory, ranked.SubCategoryId, ranked.ProductId, ranked.Product



---3.  Most Sold Under Each Category

SELECT CategoryId, Category, ProductId, Product, SUM(Quantity) AS Total_Sold
FROM (
    SELECT c.Id as CategoryId, c.Category, i.ProductId, p.Name AS Product, SUM(i.Quantity) AS Quantity,
        ROW_NUMBER() OVER (PARTITION BY c.Category ORDER BY SUM(i.Quantity) DESC) AS rank
    FROM Invoices i
        INNER JOIN Product p ON i.ProductId = p.Id
        INNER JOIN SubCategory s ON p.SubCategoryId = s.Id
        INNER JOIN Category c ON s.CategoryId = c.Id
    GROUP BY c.Category, i.ProductId, p.Name, c.Id
) ranked
WHERE rank = 1
GROUP BY CategoryId,Category, ProductId, Product;

---4.  Most Sold under Each Sub Category

SELECT s.Id AS SubCategoryId, s.Name AS SubCategory, p.Id AS ProductId, p.Name AS Product, SUM(i.Quantity) AS Total_Sold
FROM SubCategory s
    INNER JOIN Product p ON s.Id = p.SubCategoryId
    INNER JOIN Invoices i ON p.Id = i.ProductId
GROUP BY s.Id, s.Name, p.Id, p.Name
HAVING 
    SUM(i.Quantity) = (
        SELECT MAX(sub.Total_Sold)
FROM (
            SELECT s.Id AS SubCategoryId, p.Id AS ProductId, SUM(i.Quantity) AS Total_Sold
    FROM SubCategory s
        INNER JOIN Product p ON s.Id = p.SubCategoryId
        INNER JOIN Invoices i ON p.Id = i.ProductId
    GROUP BY s.Id, p.Id
        ) sub
WHERE sub.SubCategoryId = s.Id
    )

---5.  Average Order Quantity 

select i1.ProductId, AVG(i1.Quantity) as AverageOrderQuantity
from Invoices i1
where Quantity > 0
group by i1.ProductId

---6.  Repeat Customer Percentage

select repeats.ProductId, (100*repeats.noOfCustomerWhoReturned/allInvoices.numberOfCustomers) as RepeatPercentage
from (select repeats.ProductId, count(*) noOfCustomerWhoReturned
    from (select i2.ProductId, i2.CustomerId, count(*) as Repeats
        from Invoices i2
        group by i2.CustomerId, i2.ProductId
        having count(*) > 1) as repeats
    group by repeats.ProductId) as Repeats join (select i3.ProductId, count(*) numberOfCustomers
    from Invoices i3
    group by i3.ProductId
) as allInvoices on repeats.ProductId = allInvoices.ProductId

---7.  Lowest Price and Highest Price

select i4.productId, min(i4.Price) minimumPrice, max(i4.Price) maximumPrice
from Invoices i4
group by i4.productId
HAVING min(i4.Price) != max(i4.Price)


---8.  Most Sold At Location for a Product

select *
from (select i5.ProductId, i5.LOCATION, count(*) QuantitySoldAtLocation,
        ROW_NUMBER() OVER (PARTITION By i5.ProductId ORDER BY COUNT(*) desc) as rank
    from invoices i5
    group by i5.ProductId, i5.LOCATION) as ranked2
where ranked2.rank = 1

---9. Average Rating for Each Product 

Select r.ProductId, AVG(Rating) as AvgRating
from Rating r
group by r.ProductId



--- Aggregation  
SELECT p.Id,
    Isnull(avgOrders.averageorderquantity, 0) AverageOrderQuantity,
    Isnull(repeats.repeatpercentage, 0)       RepeatPercentage,
    Isnull(maxMin.minimumprice, 0)            minimumPrice,
    Isnull(maxMin.maximumprice, 0)            maximumPrice,
    Isnull(locationTable.location, '')        mostSoldAtLocation,
    Isnull(ratings.AvgRating, 0)        AvgRating

FROM product p
    LEFT JOIN
    (SELECT i1.productid,
        Avg(i1.quantity) AS AverageOrderQuantity
    FROM invoices i1
    WHERE  i1.quantity > 0
    GROUP  BY i1.productid) AS avgOrders
    on p.Id = avgOrders.productId
    LEFT JOIN (SELECT
        repeats.productid,
        ( 100 * repeats.noofcustomerwhoreturned /
                           allInvoices.numberofcustomers ) AS
                         RepeatPercentage
    FROM (SELECT repeats.productid,
            Count(*) noOfCustomerWhoReturned
        FROM (SELECT i2.productid,
                i2.customerid,
                Count(*) AS Repeats
            FROM invoices i2
            GROUP  BY i2.customerid,
                                            i2.productid
            HAVING Count(*) > 1) AS repeats
        GROUP  BY repeats.productid) AS Repeats
        JOIN (SELECT i3.productid,
            Count(*) numberOfCustomers
        FROM invoices i3
        GROUP  BY i3.productid) AS allInvoices
        ON repeats.productid = allInvoices.productid) repeats
    ON repeats.productid = avgOrders.productid
    LEFT JOIN (SELECT i4.productid,
        Min(i4.price) minimumPrice,
        Max(i4.price) maximumPrice
    FROM invoices i4
    GROUP  BY i4.productid
    HAVING Min(i4.price) != Max(i4.price)) maxMin
    ON maxMin.productid = avgOrders.productid
    LEFT JOIN (SELECT *
    FROM (SELECT i5.productid,
            i5.location,
            Count(*)
                                 QuantitySoldAtLocation,
            Row_number()
                                   OVER (
                                     partition BY i5.productid
                                     ORDER BY Count(*) DESC) AS rank
        FROM invoices i5
        GROUP  BY i5.productid,
                                    i5.location) AS ranked2
    WHERE  ranked2.rank = 1) locationTable
    ON locationTable.productid = avgOrders.productid
    LEFT JOIN
    (Select r.ProductId, AVG(r.Rating) as AvgRating
    from Rating r
    group by r.ProductId) as ratings
    on p.Id = ratings.ProductId


--- Insert into Product Analysis
insert into ProductAnalysis
SELECT p.Id,
    Isnull(avgOrders.averageorderquantity, 0) AverageOrderQuantity,
    Isnull(repeats.repeatpercentage, 0)       RepeatPercentage,
    Isnull(maxMin.minimumprice, 0)            minimumPrice,
    Isnull(maxMin.maximumprice, 0)            maximumPrice,
    Isnull(locationTable.location, '')        mostSoldAtLocation,
    Isnull(ratings.AvgRating, 0)        AvgRating
FROM product p
    LEFT JOIN
    (SELECT i1.productid,
        Avg(i1.quantity) AS AverageOrderQuantity
    FROM invoices i1
    WHERE  i1.quantity > 0
    GROUP  BY i1.productid) AS avgOrders
    on p.Id = avgOrders.productId
    LEFT JOIN (SELECT
        repeats.productid,
        ( 100 * repeats.noofcustomerwhoreturned /
                           allInvoices.numberofcustomers ) AS
                         RepeatPercentage
    FROM (SELECT repeats.productid,
            Count(*) noOfCustomerWhoReturned
        FROM (SELECT i2.productid,
                i2.customerid,
                Count(*) AS Repeats
            FROM invoices i2
            GROUP  BY i2.customerid,
                                            i2.productid
            HAVING Count(*) > 1) AS repeats
        GROUP  BY repeats.productid) AS Repeats
        JOIN (SELECT i3.productid,
            Count(*) numberOfCustomers
        FROM invoices i3
        GROUP  BY i3.productid) AS allInvoices
        ON repeats.productid = allInvoices.productid) repeats
    ON repeats.productid = avgOrders.productid
    LEFT JOIN (SELECT i4.productid,
        Min(i4.price) minimumPrice,
        Max(i4.price) maximumPrice
    FROM invoices i4
    GROUP  BY i4.productid
    HAVING Min(i4.price) != Max(i4.price)) maxMin
    ON maxMin.productid = avgOrders.productid
    LEFT JOIN (SELECT *
    FROM (SELECT i5.productid,
            i5.location,
            Count(*)
                                 QuantitySoldAtLocation,
            Row_number()
                                   OVER (
                                     partition BY i5.productid
                                     ORDER BY Count(*) DESC) AS rank
        FROM invoices i5
        GROUP  BY i5.productid,
                                    i5.location) AS ranked2
    WHERE  ranked2.rank = 1) locationTable
    ON locationTable.productid = avgOrders.productid
    LEFT JOIN
    (Select r.ProductId, AVG(r.Rating) as AvgRating
    from Rating r
    group by r.ProductId) as ratings
    on p.Id = ratings.ProductId


--- Category Join 

select CatRatings.Category_Id,
 CatRatings.ProductId as HighestRatedProductId,
  CatRatings.Product as HighestRatedProduct,
    isNull(CatRatings.Average_Rating,0) as Average_Rating,
    mostSold.ProductId as MostSoldProductId,
     mostSold.Product as MostSoldProduct, 
     isNull(mostSold.Total_Sold,0) as Total_Sold
from Category cat
    Left Join
    (SELECT ranked.Category_Id, ranked.Category, ranked.Subcategory,ranked.ProductId as ProductId, ranked.Product, AVG(ranked.Rating) AS Average_Rating
    FROM (
    SELECT c.Id as Category_Id, c.Category, s.Name AS Subcategory,p.Id as ProductId, p.Name AS Product, AVG(r.Rating) AS Rating,
            ROW_NUMBER() OVER (PARTITION BY c.Category ORDER BY AVG(r.Rating) DESC) AS rank
        FROM Category c
            INNER JOIN SubCategory s ON c.Id = s.CategoryId
            INNER JOIN Product p ON s.Id = p.SubCategoryId
            INNER JOIN Rating r ON p.Id = r.ProductId
        GROUP BY c.Category, s.Name, p.Name, c.Id, p.Id
) ranked
    WHERE ranked.rank = 1
    GROUP BY ranked.Category, ranked.Subcategory, ranked.Product,ranked.Category_Id, ranked.ProductId) as CatRatings
    on CatRatings.Category_Id = cat.Id
    LEFT JOIN
    (SELECT ranked2.CategoryId, ranked2.Category, ranked2.ProductId, ranked2.Product, SUM(ranked2.Quantity) AS Total_Sold
    FROM (
    SELECT c.Id as CategoryId, c.Category, i.ProductId, p.Name AS Product, SUM(i.Quantity) AS Quantity,
            ROW_NUMBER() OVER (PARTITION BY c.Category ORDER BY SUM(i.Quantity) DESC) AS rank
        FROM Invoices i
            INNER JOIN Product p ON i.ProductId = p.Id
            INNER JOIN SubCategory s ON p.SubCategoryId = s.Id
            INNER JOIN Category c ON s.CategoryId = c.Id
        GROUP BY c.Category, i.ProductId, p.Name, c.Id
) ranked2
    WHERE ranked2.rank = 1
    GROUP BY ranked2.CategoryId,ranked2.Category, ranked2.ProductId, ranked2.Product) as mostSold
    on mostSold.CategoryId = cat.Id


--- Sub Category Join

select subCat.Id, SubRating.ProductId as HighestRatedProductId, SubRating.Product as HighestRatedProduct,
 isNull(SubRating.Average_Rating, 0) as Average_Rating, mostSold.ProductId as MostSoldProductId, mostSold.Product as MostSoldProduct,
    isNull(mostSold.Total_Sold,0) as Total_Sold
from SubCategory subCat
    LEFT JOIN
    (SELECT ranked.Subcategory, ranked.SubCategoryId, ranked.ProductId, ranked.Product, AVG(ranked.Rating) AS Average_Rating
    FROM (
    SELECT s.Name AS Subcategory, p.SubCategoryId, p.Id AS ProductId, p.Name AS Product, AVG(r.Rating) AS Rating,
            ROW_NUMBER() OVER (PARTITION BY s.Id ORDER BY AVG(r.Rating) DESC) AS rank
        FROM SubCategory s
            INNER JOIN Product p ON s.Id = p.SubCategoryId
            INNER JOIN Rating r ON p.Id = r.ProductId
        GROUP BY s.Id, s.Name, p.SubCategoryId, p.Id, p.Name
) ranked
    WHERE ranked.rank = 1
    GROUP BY ranked.Subcategory, ranked.SubCategoryId, ranked.ProductId, ranked.Product) as SubRating
    on SubRating.SubCategoryId = subCat.Id
    LEFT JOIN
    (
SELECT s.Id AS SubCategoryId, s.Name AS SubCategory, p.Id AS ProductId, p.Name AS Product, SUM(i.Quantity) AS Total_Sold
    FROM SubCategory s
        INNER JOIN Product p ON s.Id = p.SubCategoryId
        INNER JOIN Invoices i ON p.Id = i.ProductId
    GROUP BY s.Id, s.Name, p.Id, p.Name
    HAVING 
    SUM(i.Quantity) = (
        SELECT MAX(sub.Total_Sold)
    FROM (
            SELECT s.Id AS SubCategoryId, p.Id AS ProductId, SUM(i.Quantity) AS Total_Sold
        FROM SubCategory s
            INNER JOIN Product p ON s.Id = p.SubCategoryId
            INNER JOIN Invoices i ON p.Id = i.ProductId
        GROUP BY s.Id, p.Id
        ) sub
    WHERE sub.SubCategoryId = s.Id
    )
) as mostSold
    on mostSold.SubCategoryId = subCat.Id






