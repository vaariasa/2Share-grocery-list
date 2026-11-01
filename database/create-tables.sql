/*
    Grocery List - database schema
    ------------------------------
    Creates the tables used by the backend API.

    The script is idempotent: it only creates what does not exist yet,
    so it is safe to run it more than once.

    How to run (LocalDB example):
        sqlcmd -S "(localdb)\GroceryProject" -E -i database/create-tables.sql
*/

IF DB_ID(N'GroceryProject') IS NULL
    CREATE DATABASE GroceryProject;
GO

USE GroceryProject;
GO

/* Users: people who can log in. Passwords are never stored in plain text, only a hash. */
IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users
    (
        Id           UNIQUEIDENTIFIER NOT NULL,
        Username     NVARCHAR(50)     NOT NULL,
        FullName     NVARCHAR(100)    NOT NULL,
        PasswordHash NVARCHAR(256)    NOT NULL,
        CreatedAt    DATETIMEOFFSET   NOT NULL,

        CONSTRAINT PK_Users PRIMARY KEY (Id)
    );

    -- A username can only be registered once.
    CREATE UNIQUE INDEX IX_Users_Username ON dbo.Users (Username);
END
GO

/* GroceryItems: the products on a user's grocery list. Each item belongs to exactly one user. */
IF OBJECT_ID(N'dbo.GroceryItems', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.GroceryItems
    (
        Id         UNIQUEIDENTIFIER NOT NULL,
        UserId     UNIQUEIDENTIFIER NOT NULL,
        Name       NVARCHAR(100)    NOT NULL,
        Quantity   INT              NOT NULL,
        IsInBasket BIT              NOT NULL,   -- 1 = tagged (in the basket), 0 = still to buy
        CreatedAt  DATETIMEOFFSET   NOT NULL,
        UpdatedAt  DATETIMEOFFSET   NOT NULL,

        CONSTRAINT PK_GroceryItems PRIMARY KEY (Id),
        -- Deleting a user also deletes their grocery items.
        CONSTRAINT FK_GroceryItems_Users_UserId FOREIGN KEY (UserId)
            REFERENCES dbo.Users (Id) ON DELETE CASCADE
    );

    -- Speeds up "get the list of this user, ordered by creation date".
    CREATE INDEX IX_GroceryItems_UserId_CreatedAt ON dbo.GroceryItems (UserId, CreatedAt);
END
GO
