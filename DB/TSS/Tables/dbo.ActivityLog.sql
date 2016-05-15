CREATE TABLE [dbo].[ActivityLog](
	[LogID]     [int] IDENTITY(1,1)  NOT NULL,
	[LogDate]   [datetime]           NOT NULL,
	[Category]  [int]                NOT NULL,
	[Level]     [int]                NOT NULL,
	[Message]   [nvarchar](max)      NOT NULL,
	[IpAddress] [nvarchar](max)      NOT NULL,
	[Details] [nvarchar](max)        NULL,
 CONSTRAINT [PK__ActivityLog] PRIMARY KEY CLUSTERED ([LogID] ASC)
);