CREATE DATABASE SmagerUpCore;
GO
USE SmagerUpCore;
GO

USE [SmagerUpCore]
GO

/****** Object:  Table [dbo].[Modules]    Script Date: 11/3/2025 9:45:31 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Modules](
	[ModuleId] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Version] [nvarchar](20) NOT NULL,
	[LicenseTypeId] [int] NOT NULL,
	[Price] [decimal](10, 2) NOT NULL,
	[ContentGroupId] [uniqueidentifier] NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
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

ALTER TABLE [dbo].[Modules] ADD  CONSTRAINT [DF__Modules__CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt]
GO

ALTER TABLE [dbo].[Modules]  WITH CHECK ADD  CONSTRAINT [FK_Modules_ContentGroups] FOREIGN KEY([ContentGroupId])
REFERENCES [dbo].[ContentGroups] ([ContentGroupId])
GO

ALTER TABLE [dbo].[Modules] CHECK CONSTRAINT [FK_Modules_ContentGroups]
GO

ALTER TABLE [dbo].[Modules]  WITH CHECK ADD  CONSTRAINT [FK_Modules_LicenseTypes] FOREIGN KEY([LicenseTypeId])
REFERENCES [dbo].[LicenseTypes ] ([LicenseTypeId])
GO

ALTER TABLE [dbo].[Modules] CHECK CONSTRAINT [FK_Modules_LicenseTypes]
GO


/****** Object:  Table [dbo].[Accounts]    Script Date: 11/3/2025 9:43:29 PM ******/
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
	[CreatedDate] [datetime] NULL,
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

ALTER TABLE [dbo].[Accounts] ADD  CONSTRAINT [DF__Accounts__CreateAt]  DEFAULT (getdate()) FOR [CreatedDate]
GO

/****** Object:  Table [dbo].[AccountModules]    Script Date: 11/3/2025 9:43:11 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AccountModules](
	[AccountModuleId] [uniqueidentifier] NOT NULL,
	[AccountId] [uniqueidentifier] NOT NULL,
	[ModuleId] [uniqueidentifier] NULL,
	[ExpiryDate] [datetime] NULL,
	[IsActive] [bit] NULL,
	[CreatedAt] [datetime] NULL,
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

/****** Object:  Table [dbo].[LicenseTypes ]    Script Date: 11/3/2025 9:45:20 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[LicenseTypes ](
	[LicenseTypeId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Type] [nvarchar](50) NOT NULL,
	[BodyContent] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
 CONSTRAINT [PK__LicenseTypes] PRIMARY KEY CLUSTERED 
(
	[LicenseTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[LicenseTypes ] ADD  CONSTRAINT [DF_LicenseTypes _CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt]
GO

/****** Object:  Table [dbo].[Contents]    Script Date: 11/3/2025 9:43:48 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Contents](
	[ContentId] [uniqueidentifier] NOT NULL,
	[ContetBody] [nvarchar](max) NOT NULL,
	[ContentType] [varchar](5) NULL,
	[CreatedAt] [datetime] NULL,
 CONSTRAINT [PK__Contents] PRIMARY KEY CLUSTERED 
(
	[ContentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Contents] ADD  CONSTRAINT [DF__Contents__ContentId]  DEFAULT (newid()) FOR [ContentId]
GO

ALTER TABLE [dbo].[Contents] ADD  CONSTRAINT [DF__Contents__CreateAt]  DEFAULT (getdate()) FOR [CreatedAt]
GO

ALTER TABLE [dbo].[Contents]  WITH CHECK ADD  CONSTRAINT [FK_Contents_Contents] FOREIGN KEY([ContentId])
REFERENCES [dbo].[Contents] ([ContentId])
GO

ALTER TABLE [dbo].[Contents] CHECK CONSTRAINT [FK_Contents_Contents]
GO


/****** Object:  Table [dbo].[ContentGroups]    Script Date: 11/3/2025 9:43:38 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ContentGroups](
	[ContentGroupId] [uniqueidentifier] NOT NULL,
	[ContentId] [uniqueidentifier] NULL,
	[CreatedAt] [datetime] NULL,
 CONSTRAINT [PK__ContentGroup] PRIMARY KEY CLUSTERED 
(
	[ContentGroupId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ContentGroups] ADD  CONSTRAINT [DF__ContentGr__ContentGroupId]  DEFAULT (newid()) FOR [ContentGroupId]
GO

ALTER TABLE [dbo].[ContentGroups] ADD  CONSTRAINT [DF__ContentGr__CreateAt]  DEFAULT (getdate()) FOR [CreatedAt]
GO

ALTER TABLE [dbo].[ContentGroups]  WITH CHECK ADD  CONSTRAINT [FK_ContentGroups_Contents] FOREIGN KEY([ContentId])
REFERENCES [dbo].[Contents] ([ContentId])
GO

ALTER TABLE [dbo].[ContentGroups] CHECK CONSTRAINT [FK_ContentGroups_Contents]
GO





