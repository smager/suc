CREATE DATABASE SmagerUpCore;
GO
USE SmagerUpCore;
GO

CREATE TABLE LicenseTypes (
    LicenseTypeId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL,              -- e.g. "Trial", "Pro", "Enterprise"
    Type NVARCHAR(50) NOT NULL,              -- e.g. "Subscription", "Lifetime", etc.
    BodyContent NVARCHAR(MAX) NULL,          -- full license text or HTML content
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
);


CREATE TABLE [dbo].[Modules] (
    [ModuleId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),        -- Primary Key
    [Name] NVARCHAR(100) NOT NULL,                               -- Module name (e.g., "grid", "chart")
    [Version] NVARCHAR(20) NOT NULL,                             -- Version string (e.g., "1.0.0")
    [LicenseTypeId] INT NOT NULL,                                -- FK to LicenseTypes table
    [Price] DECIMAL(10,2) NOT NULL DEFAULT 0.00,                 -- 0 = Free
    [ContentGroupId] UNIQUEIDENTIFIER NULL,                      -- FK to ContentGroups
    [IsActive] BIT NOT NULL DEFAULT 1,                           -- Active status
    [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),             -- Creation timestamp

    CONSTRAINT [PK_Modules] PRIMARY KEY CLUSTERED ([ModuleId] ASC),

    CONSTRAINT [FK_Modules_LicenseTypes]
        FOREIGN KEY ([LicenseTypeId]) REFERENCES [dbo].[LicenseTypes]([LicenseTypeId]),

    CONSTRAINT [FK_Modules_ContentGroups]
        FOREIGN KEY ([ContentGroupId]) REFERENCES [dbo].[ContentGroups]([ContentGroupId])
);

-- Ensure fast lookups for name and version combinations
CREATE UNIQUE INDEX IX_Modules_Name_Version
ON [dbo].[Modules] ([Name], [Version]);

-- Optional: for filtering by license type (e.g., getting all free modules)
CREATE INDEX IX_Modules_LicenseTypeId
ON [dbo].[Modules] ([LicenseTypeId]);


CREATE TABLE Accounts (
    AccountId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NULL,
    EmailAdd NVARCHAR(255) NULL,
    ApiKey NVARCHAR(255) NOT NULL,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT UQ_Accounts_ApiKey UNIQUE (ApiKey)
);

CREATE TABLE AccountLicenses (
    AccountLicenseId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    AccountId UNIQUEIDENTIFIER NOT NULL,
    LicenseTypeId INT NOT NULL,
    ExpiryDate DATETIME NULL,
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_AccountLicenses_Accounts FOREIGN KEY (AccountId)
        REFERENCES Accounts(AccountId),
    CONSTRAINT FK_AccountLicenses_LicenseTypes FOREIGN KEY (LicenseTypeId)
        REFERENCES LicenseTypes(LicenseTypeId)
);


CREATE TABLE UserModules (
    UserModuleId INT IDENTITY(1,1) PRIMARY KEY,
    AccountId UNIQUEIDENTIFIER NOT NULL,
    ModuleId INT NOT NULL,
    IsActive BIT DEFAULT 1,
    FOREIGN KEY (AccountId) REFERENCES Accounts(AccountId),
    FOREIGN KEY (ModuleId) REFERENCES Modules(ModuleId)
);
