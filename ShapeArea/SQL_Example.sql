CREATE TABLE Product (
  Id INT PRIMARY KEY,
  Name NVARCHAR(200) NOT NULL DEFAULT('')
);

CREATE TABLE Category (
  Id INT PRIMARY KEY,
  Name NVARCHAR(200) NOT NULL DEFAULT('')
);


CREATE TABLE ProductCategory (
  Id INT PRIMARY KEY,
  ProductId INT NOT NULL,
  CategoryId INT NOT NULL,
  FOREIGN KEY (ProductId) REFERENCES Product(Id),
  FOREIGN KEY (CategoryId) REFERENCES Category(Id)
);

SELECT Product.Name, isnull(Category.Name, '') FROM Product p
  left join ProductCategory pc on p.Id = pc.ProductId
  LEFT JOIN Category c on pc.CategoryId = c.Id