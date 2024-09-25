/*
--------------------------------------------------------------------
Name   : EBikeCommerce
Link   : https://github.com/Paol0B/EbikeCommerce
Version: 1.0
--------------------------------------------------------------------
*/

CREATE DATABASE EBikeCommerce;
GO

USE EBikeCommerce;
GO

-- Create tables with IF NOT EXISTS checks
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'categories')
BEGIN
    CREATE TABLE [dbo].[categories] (
        [category_id]   INT           IDENTITY (1, 1) NOT NULL,
        [category_name] VARCHAR (255) NOT NULL,
        PRIMARY KEY CLUSTERED ([category_id] ASC)
    );
    PRINT 'Table categories created successfully.';
END
ELSE
BEGIN
    PRINT 'Table categories already exists.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'brands')
BEGIN
    CREATE TABLE [dbo].[brands] (
        [brand_id]   INT           IDENTITY (1, 1) NOT NULL,
        [brand_name] VARCHAR (255) NOT NULL,
        PRIMARY KEY CLUSTERED ([brand_id] ASC)
    );
    PRINT 'Table brands created successfully.';
END
ELSE
BEGIN
    PRINT 'Table brands already exists.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'products')
BEGIN
    CREATE TABLE [dbo].[products] (
        [product_id]   INT             IDENTITY (1, 1) NOT NULL,
        [product_name] VARCHAR (255)   NOT NULL,
        [brand_id]     INT             NOT NULL,
        [category_id]  INT             NOT NULL,
        [model_year]   SMALLINT        NOT NULL,
        [list_price]   DECIMAL (10, 2) NOT NULL,
        PRIMARY KEY CLUSTERED ([product_id] ASC),
        FOREIGN KEY ([brand_id]) REFERENCES [dbo].[brands] ([brand_id]) ON DELETE CASCADE ON UPDATE CASCADE,
        FOREIGN KEY ([category_id]) REFERENCES [dbo].[categories] ([category_id]) ON DELETE CASCADE ON UPDATE CASCADE
    );
    PRINT 'Table products created successfully.';
END
ELSE
BEGIN
    PRINT 'Table products already exists.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'customers')
BEGIN
    CREATE TABLE [dbo].[customers] (
        [customer_id] INT           IDENTITY (1, 1) NOT NULL,
        [first_name]  VARCHAR (255) NOT NULL,
        [last_name]   VARCHAR (255) NOT NULL,
        [phone]       VARCHAR (20)  NULL,
        [username]    VARCHAR (255) NOT NULL,
        [email]       VARCHAR (255) NOT NULL,
        [passwd]      VARCHAR (255) NOT NULL,
        [street]      VARCHAR (255) NULL,
        [city]        VARCHAR (255) NULL,
        [state]       VARCHAR (255) NULL,
        [zip_code]    VARCHAR (20)  NULL,
        [mfa]         VARCHAR (255) NULL,
        PRIMARY KEY CLUSTERED ([customer_id] ASC)
    );
    PRINT 'Table customers created successfully.';
END
ELSE
BEGIN
    PRINT 'Table customers already exists.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Carts')
BEGIN
    CREATE TABLE [dbo].[Carts] (
        [cart_id]     INT NOT NULL,
        [customer_id] INT NULL,
        [product_id]  INT NULL,
        [quantity]    INT NULL,
        PRIMARY KEY CLUSTERED ([cart_id] ASC),
        FOREIGN KEY ([customer_id]) REFERENCES [dbo].[customers] ([customer_id]),
        FOREIGN KEY ([product_id]) REFERENCES [dbo].[products] ([product_id])
    );
    PRINT 'Table Carts created successfully.';
END
ELSE
BEGIN
    PRINT 'Table Carts already exists.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'stores')
BEGIN
    CREATE TABLE [dbo].[stores] (
        [store_id]   INT           IDENTITY (1, 1) NOT NULL,
        [store_name] VARCHAR (255) NOT NULL,
        [phone]      VARCHAR (25)  NULL,
        [email]      VARCHAR (255) NULL,
        [street]     VARCHAR (255) NULL,
        [city]       VARCHAR (255) NULL,
        [state]      VARCHAR (10)  NULL,
        [zip_code]   VARCHAR (5)   NULL,
        PRIMARY KEY CLUSTERED ([store_id] ASC)
    );
    PRINT 'Table stores created successfully.';
END
ELSE
BEGIN
    PRINT 'Table stores already exists.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'staffs')
BEGIN
    CREATE TABLE [dbo].[staffs] (
        [staff_id]   INT           IDENTITY (1, 1) NOT NULL,
        [first_name] VARCHAR (50)  NOT NULL,
        [last_name]  VARCHAR (50)  NOT NULL,
        [email]      VARCHAR (255) NOT NULL,
        [phone]      VARCHAR (25)  NULL,
        [active]     TINYINT       NOT NULL,
        [store_id]   INT           NOT NULL,
        [manager_id] INT           NULL,
        PRIMARY KEY CLUSTERED ([staff_id] ASC),
        FOREIGN KEY ([manager_id]) REFERENCES [dbo].[staffs] ([staff_id]),
        FOREIGN KEY ([store_id]) REFERENCES [dbo].[stores] ([store_id]) ON DELETE CASCADE ON UPDATE CASCADE,
        UNIQUE NONCLUSTERED ([email] ASC)
    );
    PRINT 'Table staffs created successfully.';
END
ELSE
BEGIN
    PRINT 'Table staffs already exists.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'orders')
BEGIN
    CREATE TABLE [dbo].[orders] (
        [order_id]      INT     IDENTITY (1, 1) NOT NULL,
        [customer_id]   INT     NULL,
        [order_status]  TINYINT NOT NULL,
        [order_date]    DATE    NOT NULL,
        [required_date] DATE    NOT NULL,
        [shipped_date]  DATE    NULL,
        [store_id]      INT     NOT NULL,
        [staff_id]      INT     NOT NULL,
        PRIMARY KEY CLUSTERED ([order_id] ASC),
        FOREIGN KEY ([customer_id]) REFERENCES [dbo].[customers] ([customer_id]) ON DELETE CASCADE ON UPDATE CASCADE,
        FOREIGN KEY ([staff_id]) REFERENCES [dbo].[staffs] ([staff_id]),
        FOREIGN KEY ([store_id]) REFERENCES [dbo].[stores] ([store_id]) ON DELETE CASCADE ON UPDATE CASCADE
    );
    PRINT 'Table orders created successfully.';
END
ELSE
BEGIN
    PRINT 'Table orders already exists.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'order_items')
BEGIN
    CREATE TABLE [dbo].[order_items] (
        [order_id]   INT             NOT NULL,
        [product_id] INT             NOT NULL,
        [quantity]   INT             NOT NULL,
        [list_price] DECIMAL (10, 2) NOT NULL,
        [discount]   DECIMAL (4, 2)  DEFAULT ((0)) NOT NULL,
        PRIMARY KEY CLUSTERED ([order_id] ASC),
        FOREIGN KEY ([order_id]) REFERENCES [dbo].[orders] ([order_id]) ON DELETE CASCADE ON UPDATE CASCADE,
        FOREIGN KEY ([product_id]) REFERENCES [dbo].[products] ([product_id]) ON DELETE CASCADE ON UPDATE CASCADE
    );
    PRINT 'Table order_items created successfully.';
END
ELSE
BEGIN
    PRINT 'Table order_items already exists.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'stocks')
BEGIN
    CREATE TABLE [dbo].[stocks] (
        [store_id]   INT NOT NULL,
        [product_id] INT NOT NULL,
        [quantity]   INT NULL,
        PRIMARY KEY CLUSTERED ([store_id] ASC, [product_id] ASC),
        FOREIGN KEY ([product_id]) REFERENCES [dbo].[products] ([product_id]) ON DELETE CASCADE ON UPDATE CASCADE,
        FOREIGN KEY ([store_id]) REFERENCES [dbo].[stores] ([store_id]) ON DELETE CASCADE ON UPDATE CASCADE
    );
    PRINT 'Table stocks created successfully.';
END
ELSE
BEGIN
    PRINT 'Table stocks already exists.';
END
GO
