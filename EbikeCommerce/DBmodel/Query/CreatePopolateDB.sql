-- Create the database
CREATE DATABASE EbikeCommerce;
USE EbikeCommerce;

-- Create the tables
CREATE TABLE Brands (
    brand_id INT PRIMARY KEY,
    brand_name VARCHAR(255) NOT NULL
);

CREATE TABLE Categories (
    category_id INT PRIMARY KEY,
    category_name VARCHAR(255) NOT NULL
);

CREATE TABLE Products (
    product_id INT PRIMARY KEY,
    product_name VARCHAR(255) NOT NULL,
    brand_id INT,
    category_id INT,
    model_year SMALLINT CHECK (model_year BETWEEN 0 AND 9999),
    list_price DECIMAL(10, 2),
    FOREIGN KEY (brand_id) REFERENCES Brands(brand_id),
    FOREIGN KEY (category_id) REFERENCES Categories(category_id)
);

CREATE TABLE Customers (
    customer_id INT IDENTITY(1,1) PRIMARY KEY,
    first_name VARCHAR(255) NOT NULL,
    last_name VARCHAR(255) NOT NULL,
    phone VARCHAR(20),
    username VARCHAR(255) NOT NULL,
    email VARCHAR(255) NOT NULL,
    passwd VARCHAR(255) NOT NULL,
    street VARCHAR(255),
    city VARCHAR(255),
    state VARCHAR(255),
    zip_code VARCHAR(20),
    mfa VARCHAR(255)
);

CREATE TABLE Carts (
    cart_id INT PRIMARY KEY,
    customer_id INT,
    product_id INT,
    quantity INT,
    FOREIGN KEY (customer_id) REFERENCES Customers(customer_id),
    FOREIGN KEY (product_id) REFERENCES Products(product_id)
);

CREATE TABLE Orders (
    order_id INT PRIMARY KEY,
    customer_id INT,
    order_date DATETIME,
    total DECIMAL(10, 2),
    FOREIGN KEY (customer_id) REFERENCES Customers(customer_id)
);

CREATE TABLE CreditCards (
    card_number VARCHAR(20) PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    expiration_month INT,
    expiration_year INT,
    cvv VARCHAR(4)
);

-- Populate the Brands table
INSERT INTO Brands (brand_id, brand_name) VALUES
(1, 'Brand A'),
(2, 'Brand B'),
(3, 'Brand C'),
(4, 'Brand D'),
(5, 'Brand E');

-- Populate the Categories table
INSERT INTO Categories (category_id, category_name) VALUES
(1, 'Mountain Bikes'),
(2, 'Road Bikes'),
(3, 'Hybrid Bikes'),
(4, 'Electric Bikes'),
(5, 'Kids Bikes');

-- Populate the Products table
INSERT INTO Products (product_id, product_name, brand_id, category_id, model_year, list_price) VALUES
(1, 'Mountain Bike X', 1, 1, 2023, 1200.00),
(2, 'Road Bike Y', 2, 2, 2022, 1500.00),
(3, 'Hybrid Bike Z', 3, 3, 2021, 800.00),
(4, 'Electric Bike A', 4, 4, 2024, 2500.00),
(5, 'Kids Bike B', 5, 5, 2020, 300.00),
(6, 'Mountain Bike C', 1, 1, 2023, 1300.00),
(7, 'Road Bike D', 2, 2, 2022, 1600.00),
(8, 'Hybrid Bike E', 3, 3, 2021, 850.00),
(9, 'Electric Bike F', 4, 4, 2024, 2600.00),
(10, 'Kids Bike G', 5, 5, 2020, 350.00);

-- Populate the Customers table
INSERT INTO Customers (customer_id, first_name, last_name, phone, username, email, passwd, street, city, state, zip_code, mfa) VALUES
(1, 'John', 'Doe', '1234567890', 'johndoe', 'john@example.com', 'password123', '123 Main St', 'Turin', 'Piedmont', '10100', 'enabled'),
(2, 'Jane', 'Smith', '0987654321', 'janesmith', 'jane@example.com', 'password456', '456 Elm St', 'Turin', 'Piedmont', '10101', 'enabled'),
(3, 'Alice', 'Johnson', '1122334455', 'alicej', 'alice@example.com', 'password789', '789 Oak St', 'Turin', 'Piedmont', '10102', 'enabled'),
(4, 'Bob', 'Brown', '2233445566', 'bobbrown', 'bob@example.com', 'password101', '101 Pine St', 'Turin', 'Piedmont', '10103', 'enabled'),
(5, 'Charlie', 'Davis', '3344556677', 'charlied', 'charlie@example.com', 'password202', '202 Maple St', 'Turin', 'Piedmont', '10104', 'enabled');

-- Populate the Carts table
INSERT INTO Carts (cart_id, customer_id, product_id, quantity) VALUES
(1, 1, 1, 2),
(2, 2, 2, 1),
(3, 3, 3, 3),
(4, 4, 4, 1),
(5, 5, 5, 2);

-- Populate the Orders table
INSERT INTO Orders (order_id, customer_id, order_date, total) VALUES
(1, 1, '2024-09-25 10:00:00', 2400.00),
(2, 2, '2024-09-24 11:00:00', 1500.00),
(3, 3, '2024-09-23 12:00:00', 2400.00),
(4, 4, '2024-09-22 13:00:00', 2500.00),
(5, 5, '2024-09-21 14:00:00', 600.00);

-- Populate the CreditCards table
INSERT INTO CreditCards (card_number, name, expiration_month, expiration_year, cvv) VALUES
('1234567890123456', 'John Doe', 12, 2025, '123'),
('2345678901234567', 'Jane Smith', 11, 2024, '234'),
('3456789012345678', 'Alice Johnson', 10, 2023, '345'),
('4567890123456789', 'Bob Brown', 9, 2022, '456'),
('5678901234567890', 'Charlie Davis', 8, 2021, '567');

--Query
SELECT * FROM Products