USE [ExperimentDB]
GO

/****** Object:  Table [dbo].[tblAlgorithm]    Script Date: 2023-03-09 11:32:02 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tblAlgorithm](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [dbo].[en_name] NULL,
	[Description] [dbo].[en_desc] NULL,
	[Type] [int] NULL,
	[ObjectId] [int] NULL,
	[AlarmId] [int] NULL,
	[GroupId] [int] NULL,
	[DatapointId] [int] NULL,
	[ValueFrom] [decimal](18, 4) NULL,
	[ValueTo] [decimal](18, 4) NULL,
	[DateStart] [date] NULL,
	[DateEnd] [date] NULL,
	[TimeStart] [time](7) NULL,
	[TimeEnd] [time](7) NULL,
	[OnMonday] [bit] NOT NULL,
	[OnTuesday] [bit] NOT NULL,
	[OnWednesday] [bit] NOT NULL,
	[OnThursday] [bit] NOT NULL,
	[OnFriday] [bit] NOT NULL,
	[OnSaturday] [bit] NOT NULL,
	[OnSunday] [bit] NOT NULL,
	[ValueOff] [decimal](18, 4) NOT NULL,
	[ValueOn] [decimal](18, 4) NOT NULL,
	[Status] [decimal](18, 4) NOT NULL,
	[Deleted] [datetime] NULL,
 CONSTRAINT [PK_tblAlgorithm] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[tblAlgorithm] ADD  CONSTRAINT [DF_tblAlgorithm_Type]  DEFAULT ((10)) FOR [Type]
GO

ALTER TABLE [dbo].[tblAlgorithm] ADD  CONSTRAINT [DF_tblAlgorithm_ObjectId]  DEFAULT ((0)) FOR [ObjectId]
GO

ALTER TABLE [dbo].[tblAlgorithm] ADD  CONSTRAINT [DF_tblAlgorithm_AlarmId]  DEFAULT ((0)) FOR [AlarmId]
GO

ALTER TABLE [dbo].[tblAlgorithm] ADD  CONSTRAINT [DF_tblAlgorithm_GroupId]  DEFAULT ((0)) FOR [GroupId]
GO

ALTER TABLE [dbo].[tblAlgorithm] ADD  CONSTRAINT [DF_tblAlgorithm_DatapointId]  DEFAULT ((0)) FOR [DatapointId]
GO

ALTER TABLE [dbo].[tblAlgorithm] ADD  CONSTRAINT [DF_tblAlgorithm_ValueFrom_1]  DEFAULT ((0)) FOR [ValueFrom]
GO

ALTER TABLE [dbo].[tblAlgorithm] ADD  CONSTRAINT [DF_tblAlgorithm_ValueTo_1]  DEFAULT ((1)) FOR [ValueTo]
GO

ALTER TABLE [dbo].[tblAlgorithm] ADD  CONSTRAINT [DF_tblAlgorithm_DateStart_1]  DEFAULT (getdate()) FOR [DateStart]
GO

ALTER TABLE [dbo].[tblAlgorithm] ADD  CONSTRAINT [DF_tblAlgorithm_DateEnd_1]  DEFAULT (getdate()) FOR [DateEnd]
GO

ALTER TABLE [dbo].[tblAlgorithm] ADD  CONSTRAINT [DF_tblAlgorithm_TimeStart_1]  DEFAULT (getdate()) FOR [TimeStart]
GO

ALTER TABLE [dbo].[tblAlgorithm] ADD  CONSTRAINT [DF_tblAlgorithm_TimeEnd_1]  DEFAULT (getdate()) FOR [TimeEnd]
GO

ALTER TABLE [dbo].[tblAlgorithm] ADD  CONSTRAINT [DF_tblAlgorithm_OnMonday_1]  DEFAULT ((0)) FOR [OnMonday]
GO

ALTER TABLE [dbo].[tblAlgorithm] ADD  CONSTRAINT [DF_tblAlgorithm_OnTuesday_1]  DEFAULT ((0)) FOR [OnTuesday]
GO

ALTER TABLE [dbo].[tblAlgorithm] ADD  CONSTRAINT [DF_tblAlgorithm_OnWednesday_1]  DEFAULT ((0)) FOR [OnWednesday]
GO

ALTER TABLE [dbo].[tblAlgorithm] ADD  CONSTRAINT [DF_tblAlgorithm_OnThursday_1]  DEFAULT ((0)) FOR [OnThursday]
GO

ALTER TABLE [dbo].[tblAlgorithm] ADD  CONSTRAINT [DF_tblAlgorithm_OnFriday_1]  DEFAULT ((0)) FOR [OnFriday]
GO

ALTER TABLE [dbo].[tblAlgorithm] ADD  CONSTRAINT [DF_tblAlgorithm_OnSaturday_1]  DEFAULT ((0)) FOR [OnSaturday]
GO

ALTER TABLE [dbo].[tblAlgorithm] ADD  CONSTRAINT [DF_tblAlgorithm_OnSunday_1]  DEFAULT ((0)) FOR [OnSunday]
GO

ALTER TABLE [dbo].[tblAlgorithm] ADD  CONSTRAINT [DF_tblAlgorithm_ValueOff_1]  DEFAULT ((0)) FOR [ValueOff]
GO

ALTER TABLE [dbo].[tblAlgorithm] ADD  CONSTRAINT [DF_tblAlgorithm_ValueOn_1]  DEFAULT ((1)) FOR [ValueOn]
GO

ALTER TABLE [dbo].[tblAlgorithm] ADD  CONSTRAINT [DF_tblAlgorithm_Status]  DEFAULT ((0)) FOR [Status]
GO


