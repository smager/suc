CREATE DATABASE SmagerUpCore;
GO

USE [SmagerUpCore]
GO

/****** Object:  Table [dbo].[Modules]    Script Date: 11/5/2025 6:35:22 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Modules](
	[ModuleId] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Version] [nvarchar](20) NOT NULL,
	[LicenseTypeId] [uniqueidentifier] NULL,
	[Price] [decimal](10, 2) NOT NULL,
	[ContentGroupId] [uniqueidentifier] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[UpdatedAt] [datetime] NULL,
	[DeletedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Modules] PRIMARY KEY CLUSTERED 
(
	[ModuleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Modules] ADD  CONSTRAINT [DF__Modules__ModuleId]  DEFAULT (newid()) FOR [ModuleId]
GO

ALTER TABLE [dbo].[Modules] ADD  CONSTRAINT [DF__Modules__Price]  DEFAULT ((0.00)) FOR [Price]
GO

ALTER TABLE [dbo].[Modules] ADD  CONSTRAINT [DF__Modules__IsActive]  DEFAULT ((1)) FOR [IsActive]
GO

ALTER TABLE [dbo].[Modules] ADD  CONSTRAINT [DF_Modules_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO

ALTER TABLE [dbo].[Modules] ADD  CONSTRAINT [DF__Modules__CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt]
GO

ALTER TABLE [dbo].[Modules]  WITH CHECK ADD  CONSTRAINT [FK_Modules_ContentGroups] FOREIGN KEY([ContentGroupId])
REFERENCES [dbo].[ContentGroups] ([ContentGroupId])
GO

ALTER TABLE [dbo].[Modules] CHECK CONSTRAINT [FK_Modules_ContentGroups]
GO

ALTER TABLE [dbo].[Modules]  WITH CHECK ADD  CONSTRAINT [FK_Modules_LicenseTypes ] FOREIGN KEY([LicenseTypeId])
REFERENCES [dbo].[LicenseTypes ] ([LicenseTypeId])
GO

ALTER TABLE [dbo].[Modules] CHECK CONSTRAINT [FK_Modules_LicenseTypes ]
GO


CREATE OR ALTER TRIGGER [dbo].[TRG_Modules_Delete]
ON [dbo].[Modules]
INSTEAD OF DELETE
AS
BEGIN
    SET NOCOUNT ON;

    -- Step 1: Prevent delete if the module is linked to any account 
    IF EXISTS (
        SELECT 1
        FROM AccountModules am
        INNER JOIN deleted d ON am.ModuleId = d.ModuleId
    )
    BEGIN
        RAISERROR('Cannot delete this module. It is currently assigned to an account.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END

    -- Step 2: Soft delete the module
    UPDATE m
    SET m.IsDeleted = 1
    FROM Modules m
    INNER JOIN deleted d ON m.ModuleId = d.ModuleId;

    -- Step 3: Soft delete associated ContentGroups
    UPDATE cg
    SET cg.IsDeleted = 1
    FROM ContentGroups cg
    INNER JOIN deleted d ON cg.ContentGroupId = d.ContentGroupId;

    -- Step 4: Soft delete associated Contents (only if unused elsewhere)
    UPDATE c
    SET c.IsDeleted = 1
    FROM Contents c
    WHERE EXISTS (
        SELECT 1
        FROM ContentGroups cg
        INNER JOIN deleted d ON cg.ContentGroupId = d.ContentGroupId
        WHERE cg.ContentId = c.ContentId
    )
    AND NOT EXISTS (
        SELECT 1
        FROM ContentGroups cg2
        WHERE cg2.ContentId = c.ContentId
        AND cg2.IsDeleted = 0
    );
END
GO

/****** Object:  Table [dbo].[Accounts]    Script Date: 11/5/2025 6:31:16 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Accounts](
	[AccountId] [uniqueidentifier] NOT NULL,
	[FirstName] [nvarchar](100) NOT NULL,
	[LastName] [nvarchar](100) NULL,
	[EmailAdd] [nvarchar](255) NULL,
	[ApiKey] [nvarchar](255) NOT NULL,
	[IsActive] [bit] NULL,
	[IsLocked] [bit] NULL,
	[IsDeleted] [bit] NULL,
	[CreatedAt] [datetime] NOT NULL,
	[UpdatedAt] [datetime] NULL,
	[DeletedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
 CONSTRAINT [PK__Accounts__AccountId] PRIMARY KEY CLUSTERED 
(
	[AccountId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Accounts_ApiKey] UNIQUE NONCLUSTERED 
(
	[ApiKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Accounts] ADD  CONSTRAINT [DF__Accounts__AccountId]  DEFAULT (newid()) FOR [AccountId]
GO

ALTER TABLE [dbo].[Accounts] ADD  CONSTRAINT [DF_Accounts_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO

ALTER TABLE [dbo].[Accounts] ADD  CONSTRAINT [DF_Accounts_IsLocked]  DEFAULT ((0)) FOR [IsLocked]
GO

ALTER TABLE [dbo].[Accounts] ADD  CONSTRAINT [DF_Accounts_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO

ALTER TABLE [dbo].[Accounts] ADD  CONSTRAINT [DF_Accounts_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt]
GO

/****** Object:  Table [dbo].[AccountModules]    Script Date: 11/5/2025 6:29:42 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AccountModules](
	[AccountModuleId] [uniqueidentifier] NOT NULL,
	[AccountId] [uniqueidentifier] NOT NULL,
	[ModuleId] [uniqueidentifier] NULL,
	[ExpiryAt] [datetime] NULL,
	[IsActive] [bit] NULL,
	[CreatedAt] [datetime] NOT NULL,
	[UpdatedAt] [datetime] NULL,
	[DeletedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
 CONSTRAINT [PK__AccountModules] PRIMARY KEY CLUSTERED 
(
	[AccountModuleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[AccountModules] ADD  CONSTRAINT [DF__AccountMo__AccountModuleId]  DEFAULT (newid()) FOR [AccountModuleId]
GO

ALTER TABLE [dbo].[AccountModules] ADD  CONSTRAINT [DF__AccountMo__IsActive]  DEFAULT ((1)) FOR [IsActive]
GO

ALTER TABLE [dbo].[AccountModules] ADD  CONSTRAINT [DF__AccountMo__CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt]
GO

ALTER TABLE [dbo].[AccountModules]  WITH CHECK ADD  CONSTRAINT [FK_AccountModules_Accounts] FOREIGN KEY([AccountId])
REFERENCES [dbo].[Accounts] ([AccountId])
GO

ALTER TABLE [dbo].[AccountModules] CHECK CONSTRAINT [FK_AccountModules_Accounts]
GO

/****** Object:  Table [dbo].[LicenseTypes ]    Script Date: 11/5/2025 6:34:35 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[LicenseTypes ](
	[LicenseTypeId] [uniqueidentifier] NOT NULL,
	[ContentId] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NOT NULL,
	[UpdatedAt] [datetime] NULL,
	[DeletedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
 CONSTRAINT [PK_LicenseTypes ] PRIMARY KEY CLUSTERED 
(
	[LicenseTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[LicenseTypes ] ADD  CONSTRAINT [DF_LicenseTypes _LicenseTypeId]  DEFAULT (newid()) FOR [LicenseTypeId]
GO

ALTER TABLE [dbo].[LicenseTypes ] ADD  CONSTRAINT [DF_LicenseTypes _CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt]
GO

ALTER TABLE [dbo].[LicenseTypes ]  WITH CHECK ADD  CONSTRAINT [FK_LicenseTypes _LicenseTypes ] FOREIGN KEY([LicenseTypeId])
REFERENCES [dbo].[LicenseTypes ] ([LicenseTypeId])
GO

ALTER TABLE [dbo].[LicenseTypes ] CHECK CONSTRAINT [FK_LicenseTypes _LicenseTypes ]
GO

/****** Object:  Table [dbo].[Contents]    Script Date: 11/5/2025 6:33:49 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Contents](
	[ContentId] [uniqueidentifier] NOT NULL,
	[ContentName] [varchar](30) NOT NULL,
	[Title] [varchar](50) NULL,
	[Body] [nvarchar](max) NOT NULL,
	[Type] [varchar](5) NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[UpdatedAt] [datetime] NULL,
	[DeletedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
 CONSTRAINT [PK__Contents] PRIMARY KEY CLUSTERED 
(
	[ContentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [IX_Contents_ContentName] UNIQUE NONCLUSTERED 
(
	[ContentName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Contents] ADD  CONSTRAINT [DF_Contents_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO

ALTER TABLE [dbo].[Contents] ADD  CONSTRAINT [DF_Contents_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt]
GO

ALTER TABLE [dbo].[Contents]  WITH CHECK ADD  CONSTRAINT [FK_Contents_Contents] FOREIGN KEY([ContentId])
REFERENCES [dbo].[Contents] ([ContentId])
GO

ALTER TABLE [dbo].[Contents] CHECK CONSTRAINT [FK_Contents_Contents]
GO

/****** Object:  Table [dbo].[ContentGroups]    Script Date: 11/5/2025 6:32:29 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ContentGroups](
	[ContentGroupId] [uniqueidentifier] NOT NULL,
	[ContentId] [uniqueidentifier] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[DeletedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
 CONSTRAINT [PK__ContentGroup] PRIMARY KEY CLUSTERED 
(
	[ContentGroupId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ContentGroups] ADD  CONSTRAINT [DF__ContentGr__ContentGroupId]  DEFAULT (newid()) FOR [ContentGroupId]
GO

ALTER TABLE [dbo].[ContentGroups] ADD  CONSTRAINT [DF_ContentGroups_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO

ALTER TABLE [dbo].[ContentGroups] ADD  CONSTRAINT [DF_ContentGroups_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO

ALTER TABLE [dbo].[ContentGroups] ADD  CONSTRAINT [DF_ContentGroups_CreatedAt1]  DEFAULT (getdate()) FOR [CreatedAt]
GO

ALTER TABLE [dbo].[ContentGroups]  WITH CHECK ADD  CONSTRAINT [FK_ContentGroups_Contents] FOREIGN KEY([ContentId])
REFERENCES [dbo].[Contents] ([ContentId])
GO

ALTER TABLE [dbo].[ContentGroups] CHECK CONSTRAINT [FK_ContentGroups_Contents]
GO