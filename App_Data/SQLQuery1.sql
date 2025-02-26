﻿CREATE TABLE Category (
    CategoryID INT PRIMARY KEY IDENTITY (1, 1) NOT NULL,
    CategoryName NVARCHAR (MAX) NULL
);
GO

CREATE TABLE Product (
    ProductID INT PRIMARY KEY IDENTITY (1, 1) NOT NULL,
    ProductName NVARCHAR (MAX) NULL,
    CategoryID INT NOT NULL
);
GO

CREATE TABLE OrderMaster (
    OrderID INT PRIMARY KEY IDENTITY (1, 1) NOT NULL,
    OrderDate DATETIME       NOT NULL,
    Description NVARCHAR (MAX) NULL,
    AddressProofImage NVARCHAR (MAX) NULL,
    Terms BIT NULL
);
GO

CREATE TABLE OrderDetail (
    OrderDetailID INT PRIMARY KEY IDENTITY (1, 1) NOT NULL,
    ProductID INT NOT NULL,
    OrderID INT FOREIGN KEY REFERENCES OrderMaster (OrderID) ON DELETE CASCADE,
    Quantity INT NOT NULL,
    Rate DECIMAL (18, 2) NOT NULL
);
GO
