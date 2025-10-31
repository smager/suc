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


CREATE TABLE Modules (
    ModuleId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Version NVARCHAR(20) NOT NULL,
    Code NVARCHAR(MAX) NOT NULL,
    Description NVARCHAR(255),
    LicenseId INT NOT NULL,
    Price DECIMAL(10,2) DEFAULT 0,
    IsActive BIT DEFAULT 1,
    FOREIGN KEY (LicenseId) REFERENCES Licenses(LicenseId)
);

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
