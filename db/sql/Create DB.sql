USE [master]
GO
/****** Object:  Database [ExperimentDB]    Script Date: 2022-08-26 22:33:17 ******/
CREATE DATABASE [ExperimentDB]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'ExperimentDB', FILENAME = N'C:\w\db\mssql\data\ExperimentDB.mdf' , SIZE = 73728KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'ExperimentDB_log', FILENAME = N'C:\w\db\mssql\data\ExperimentDB_log.ldf' , SIZE = 139264KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [ExperimentDB] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [ExperimentDB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [ExperimentDB] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [ExperimentDB] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [ExperimentDB] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [ExperimentDB] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [ExperimentDB] SET ARITHABORT OFF 
GO
ALTER DATABASE [ExperimentDB] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [ExperimentDB] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [ExperimentDB] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [ExperimentDB] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [ExperimentDB] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [ExperimentDB] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [ExperimentDB] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [ExperimentDB] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [ExperimentDB] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [ExperimentDB] SET  DISABLE_BROKER 
GO
ALTER DATABASE [ExperimentDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [ExperimentDB] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [ExperimentDB] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [ExperimentDB] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [ExperimentDB] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [ExperimentDB] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [ExperimentDB] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [ExperimentDB] SET RECOVERY FULL 
GO
ALTER DATABASE [ExperimentDB] SET  MULTI_USER 
GO
ALTER DATABASE [ExperimentDB] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [ExperimentDB] SET DB_CHAINING OFF 
GO
ALTER DATABASE [ExperimentDB] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [ExperimentDB] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [ExperimentDB] SET DELAYED_DURABILITY = DISABLED 
GO
EXEC sys.sp_db_vardecimal_storage_format N'ExperimentDB', N'ON'
GO
ALTER DATABASE [ExperimentDB] SET QUERY_STORE = OFF
GO
USE [ExperimentDB]
GO
/****** Object:  User [IIS APPPOOL\DefaultAppPool]    Script Date: 2022-08-26 22:33:17 ******/
CREATE USER [IIS APPPOOL\DefaultAppPool] FOR LOGIN [IIS APPPOOL\DefaultAppPool] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_owner] ADD MEMBER [IIS APPPOOL\DefaultAppPool]
GO
ALTER ROLE [db_datareader] ADD MEMBER [IIS APPPOOL\DefaultAppPool]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [IIS APPPOOL\DefaultAppPool]
GO
/****** Object:  UserDefinedDataType [dbo].[en_desc]    Script Date: 2022-08-26 22:33:17 ******/
CREATE TYPE [dbo].[en_desc] FROM [ntext] NULL
GO
/****** Object:  UserDefinedDataType [dbo].[en_name]    Script Date: 2022-08-26 22:33:17 ******/
CREATE TYPE [dbo].[en_name] FROM [nvarchar](256) NOT NULL
GO
/****** Object:  UserDefinedDataType [dbo].[en_sys_name]    Script Date: 2022-08-26 22:33:17 ******/
CREATE TYPE [dbo].[en_sys_name] FROM [varchar](64) NOT NULL
GO
/****** Object:  UserDefinedFunction [dbo].[fncAggregateDate]    Script Date: 2022-08-26 22:33:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[fncAggregateDate] (
	@date datetime, 
	@measureUnit varchar(32)) 

	RETURNS SMALLDATETIME AS
BEGIN
	RETURN CASE @measureUnit
		WHEN 'Hour' THEN CAST(CONVERT(VARCHAR(13), @date, 120) +':00:00' AS SMALLDATETIME)
		WHEN 'Day' THEN CAST(CONVERT(VARCHAR(10), @date, 120) +' 00:00:00' AS SMALLDATETIME)
		WHEN 'Week' THEN CAST(CONVERT(VARCHAR(19), DATEADD(DAY, 2 - DATEPART(WEEKDAY, @date), CAST(@date AS DATE)), 120) + ' 00:00:00' AS SMALLDATETIME)
		WHEN 'Quarter' THEN CAST(CONVERT(VARCHAR(19), DATEADD(q, DATEDIFF(q, 0, @date), 0), 120) AS SMALLDATETIME)
		WHEN 'Month' THEN CAST(CONVERT(VARCHAR(7), @date, 120) +'-01 00:00:00' AS SMALLDATETIME)
		WHEN 'Year' THEN CAST(CONVERT(VARCHAR(4), @date, 120) +'-01-01 00:00:00' AS SMALLDATETIME)
		ELSE CAST(@date AS SMALLDATETIME) END
END
GO
/****** Object:  UserDefinedFunction [dbo].[fncUIWord]    Script Date: 2022-08-26 22:33:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[fncUIWord] (
	@alias varchar(64), 
	@code varchar(4)
) 
RETURNS nvarchar(max) AS BEGIN
	DECLARE @retVal nvarchar(max)
	SELECT @retVal = [text] FROM tblUiWord WHERE [alias] = @alias AND [Code] = @code
	RETURN @retVal

END
GO
/****** Object:  Table [dbo].[__MigrationHistory]    Script Date: 2022-08-26 22:33:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__MigrationHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ContextKey] [nvarchar](300) NOT NULL,
	[Model] [varbinary](max) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK_dbo.__MigrationHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC,
	[ContextKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetRoles]    Script Date: 2022-08-26 22:33:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoles](
	[Id] [nvarchar](128) NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetRoles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserClaims]    Script Date: 2022-08-26 22:33:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](128) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.AspNetUserClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserLogins]    Script Date: 2022-08-26 22:33:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserLogins](
	[LoginProvider] [nvarchar](128) NOT NULL,
	[ProviderKey] [nvarchar](128) NOT NULL,
	[UserId] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetUserLogins] PRIMARY KEY CLUSTERED 
(
	[LoginProvider] ASC,
	[ProviderKey] ASC,
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserRoles]    Script Date: 2022-08-26 22:33:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserRoles](
	[UserId] [nvarchar](128) NOT NULL,
	[RoleId] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetUserRoles] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUsers]    Script Date: 2022-08-26 22:33:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUsers](
	[Id] [nvarchar](128) NOT NULL,
	[Email] [nvarchar](256) NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[LockoutEndDateUtc] [datetime] NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
	[UserName] [nvarchar](256) NOT NULL,
	[Language] [nvarchar](3) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblDatapoint]    Script Date: 2022-08-26 22:33:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblDatapoint](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DeviceId] [int] NOT NULL,
	[Name] [dbo].[en_name] NOT NULL,
	[Description] [dbo].[en_desc] NULL,
 CONSTRAINT [PK_tblDatapoint] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblDatapointValue]    Script Date: 2022-08-26 22:33:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblDatapointValue](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DatapointId] [int] NOT NULL,
	[Date] [smalldatetime] NOT NULL,
	[Value] [decimal](18, 4) NOT NULL,
 CONSTRAINT [PK_tblDatapointValue] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblDevice]    Script Date: 2022-08-26 22:33:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblDevice](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [dbo].[en_name] NULL,
	[Description] [dbo].[en_desc] NULL,
	[Type] [int] NOT NULL,
	[CreationDate] [datetime] NULL,
	[ModificationDate] [datetime] NULL,
	[UserId] [nvarchar](128) NULL,
 CONSTRAINT [PK_tblDevice_1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblUiLanguage]    Script Date: 2022-08-26 22:33:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblUiLanguage](
	[Code] [varchar](4) NOT NULL,
	[Name] [dbo].[en_name] NULL,
 CONSTRAINT [PK_tblUiLanguage] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblUiWord]    Script Date: 2022-08-26 22:33:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblUiWord](
	[Alias] [dbo].[en_sys_name] NOT NULL,
	[Code] [varchar](4) NOT NULL,
	[Text] [nvarchar](max) NULL,
	[Autoadded] [bit] NOT NULL,
 CONSTRAINT [PK_tblUiWord] PRIMARY KEY CLUSTERED 
(
	[Alias] ASC,
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [RoleNameIndex]    Script Date: 2022-08-26 22:33:17 ******/
CREATE UNIQUE NONCLUSTERED INDEX [RoleNameIndex] ON [dbo].[AspNetRoles]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_UserId]    Script Date: 2022-08-26 22:33:17 ******/
CREATE NONCLUSTERED INDEX [IX_UserId] ON [dbo].[AspNetUserClaims]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_UserId]    Script Date: 2022-08-26 22:33:17 ******/
CREATE NONCLUSTERED INDEX [IX_UserId] ON [dbo].[AspNetUserLogins]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_RoleId]    Script Date: 2022-08-26 22:33:17 ******/
CREATE NONCLUSTERED INDEX [IX_RoleId] ON [dbo].[AspNetUserRoles]
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_UserId]    Script Date: 2022-08-26 22:33:17 ******/
CREATE NONCLUSTERED INDEX [IX_UserId] ON [dbo].[AspNetUserRoles]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UserNameIndex]    Script Date: 2022-08-26 22:33:17 ******/
CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex] ON [dbo].[AspNetUsers]
(
	[UserName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[AspNetUsers] ADD  CONSTRAINT [DF_AspNetUsers_Language]  DEFAULT ('en') FOR [Language]
GO
ALTER TABLE [dbo].[tblDatapointValue] ADD  CONSTRAINT [DF_tblDatapointValue_Date]  DEFAULT (getdate()) FOR [Date]
GO
ALTER TABLE [dbo].[tblDatapointValue] ADD  CONSTRAINT [DF_tblDatapointValue_Value]  DEFAULT ((0)) FOR [Value]
GO
ALTER TABLE [dbo].[tblDevice] ADD  CONSTRAINT [DF_tblDevice_CreationDate]  DEFAULT (getdate()) FOR [CreationDate]
GO
ALTER TABLE [dbo].[tblUiWord] ADD  CONSTRAINT [DF_tblUiWord_autoadded]  DEFAULT ((0)) FOR [Autoadded]
GO
ALTER TABLE [dbo].[AspNetUserClaims]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserClaims] CHECK CONSTRAINT [FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserLogins]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserLogins] CHECK CONSTRAINT [FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[tblDatapoint]  WITH NOCHECK ADD  CONSTRAINT [FK_tblDatapoint_tblDevice] FOREIGN KEY([DeviceId])
REFERENCES [dbo].[tblDevice] ([Id])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[tblDatapoint] NOCHECK CONSTRAINT [FK_tblDatapoint_tblDevice]
GO
ALTER TABLE [dbo].[tblDatapointValue]  WITH NOCHECK ADD  CONSTRAINT [FK_tblDatapointValue_tblDatapoint] FOREIGN KEY([DatapointId])
REFERENCES [dbo].[tblDatapoint] ([Id])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[tblDatapointValue] NOCHECK CONSTRAINT [FK_tblDatapointValue_tblDatapoint]
GO
ALTER TABLE [dbo].[tblUiWord]  WITH NOCHECK ADD  CONSTRAINT [FK_tblUiWord_tblUiLanguage] FOREIGN KEY([Code])
REFERENCES [dbo].[tblUiLanguage] ([Code])
GO
ALTER TABLE [dbo].[tblUiWord] NOCHECK CONSTRAINT [FK_tblUiWord_tblUiLanguage]
GO
/****** Object:  StoredProcedure [dbo].[prcDatapointValueList]    Script Date: 2022-08-26 22:33:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[prcDatapointValueList] (
	@dateFrom datetime,
	@dateTo datetime,
	@datapointIds varchar(max),
	@measureUnit varchar(32),
	@aggregation varchar(32)
) AS
BEGIN

	IF @measureUnit = 'Minute' BEGIN

		SELECT
			DV.[Id]
			,DV.[DatapointId]
			,DV.[Date]
			,DV.[Value]
		FROM 
			tblDatapoint DP
		INNER JOIN tblDatapointValue DV ON DV.DatapointId = DP.Id
		WHERE
			DV.[Date] BETWEEN @dateFrom AND @dateTo
		AND DP.Id IN (SELECT [value] FROM STRING_SPLIT(@datapointIds, '|'))

	END ELSE BEGIN 

		SELECT
			-- This needed for EF, as it needs unique Id, elsewhere not working
			CAST(ROW_NUMBER() OVER(
				ORDER BY 
					DP.Id
					,dbo.fncAggregateDate(DV.[Date], @measureUnit)
				) as int) [Id]
			,DP.Id [DatapointId]
			,dbo.fncAggregateDate(DV.[Date], @measureUnit) [Date]
			,CASE @aggregation
				WHEN 'MinimalValue' THEN MIN(DV.[Value])
				WHEN 'MaximumValue' THEN MAX(DV.[Value])
				WHEN 'SumValue'		THEN SUM(DV.[Value])
				WHEN 'AverageValue' THEN AVG(DV.[Value])
				ELSE AVG(DV.[Value]) -- AVG
			END [Value]
		FROM 
			tblDatapoint DP
		INNER JOIN tblDatapointValue DV ON DV.DatapointId = DP.Id

		WHERE
			DV.[Date] BETWEEN @dateFrom AND @dateTo
		AND DV.DatapointId IN (SELECT [value] FROM STRING_SPLIT(@datapointIds, '|'))

		GROUP BY
			DP.Id
			,dbo.fncAggregateDate(DV.[Date], @measureUnit)

		ORDER BY
			DP.Id
			,dbo.fncAggregateDate(DV.[Date], @measureUnit)

	END
END
GO
/****** Object:  StoredProcedure [dbo].[prcMainTree]    Script Date: 2022-08-26 22:33:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[prcMainTree] (
	@userId nvarchar(128)
) AS BEGIN

	DECLARE @DEV_P varchar(3), @DTP_P varchar(3)
	SELECT @DEV_P = 'dev', @DTP_P = 'dtp'

 SELECT 
	@DEV_P + CAST(DEV.Id AS VARCHAR) [Id]
	,CAST('#' AS VARCHAR) [Parent]
	,DEV.Name [Text]
	,@DEV_P [Type]
 FROM	
	tblDevice DEV
 WHERE
	DEV.UserId = @userId

UNION ALL

SELECT
	@DTP_P + CAST(DTP.Id AS VARCHAR) Id
	,@DEV_P + CAST(DTP.DeviceId AS VARCHAR) Parent
	,DTP.Name [Text]
	,@DTP_P [Type]
FROM tblDatapoint DTP
WHERE DTP.DeviceId IN 
	(SELECT Id FROM tblDevice WHERE UserId = @userId)


END
GO
/****** Object:  StoredProcedure [dbo].[prcUiWordAll]    Script Date: 2022-08-26 22:33:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[prcUiWordAll] AS

SELECT * 
FROM 
	tblUiWord
ORDER BY
	Code ASC, alias ASC

GO
/****** Object:  StoredProcedure [dbo].[prcUiWordRegister]    Script Date: 2022-08-26 22:33:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[prcUiWordRegister] (@alias en_sys_name) AS

IF NOT EXISTS (SELECT * FROM tblUiWords WHERE alias = @alias) BEGIN
	INSERT INTO tblUiWord (alias, Code, [text], autoadded)
	SELECT @alias, Code, Code +': '+ @alias, 1  FROM tblUiLanguage
END

GO
/****** Object:  StoredProcedure [dbo].[prcUiWordsByLanguage]    Script Date: 2022-08-26 22:33:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[prcUiWordsByLanguage] (@code varchar(4)) AS

SELECT 
	UW.[alias],
	UW.[text]
FROM 
	tblUiWord UW
WHERE 
	UW.Code = @code
GO
/****** Object:  StoredProcedure [dbo].[prcUiWordUpdate]    Script Date: 2022-08-26 22:33:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[prcUiWordUpdate] (
	@alias en_sys_name, 
	@en_text nvarchar(max),
	@lt_text nvarchar(max),
	@ru_text nvarchar(max)) AS BEGIN

BEGIN TRAN

	IF EXISTS (SELECT * FROM tblUiWord WHERE [alias] = @alias) BEGIN
		UPDATE tblUiWord SET [text] = @en_text, [autoadded] = 0 WHERE [alias] = @alias AND [Code] = 'en'
		UPDATE tblUiWord SET [text] = @lt_text, [autoadded] = 0 WHERE [alias] = @alias AND [Code] = 'lt'
		UPDATE tblUiWord SET [text] = @ru_text, [autoadded] = 0 WHERE [alias] = @alias AND [Code] = 'ru'
	END ELSE BEGIN
		INSERT INTO tblUiWord 
			([alias], [Code], [text], [autoadded])
		VALUES
			(@alias, 'en', @en_text, 0),
			(@alias, 'lt', @lt_text, 0),
			(@alias, 'ru', @ru_text, 0)
	END

COMMIT TRAN
END

GO
USE [master]
GO
ALTER DATABASE [ExperimentDB] SET  READ_WRITE 
GO
