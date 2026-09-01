USE [ExperimentDB]
GO

/****** Object:  Table [dbo].[tblVars]    Script Date: 2022-08-25 10:29:36 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tblVars](
	[name] [dbo].[en_sys_name] NOT NULL,
	[value] [ntext] NULL,
	[module] [dbo].[en_sys_name] NULL,
	[datatype] [dbo].[en_name] NULL,
	[desc] [ntext] NULL,
 CONSTRAINT [PK_tblVars] PRIMARY KEY CLUSTERED 
(
	[name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
