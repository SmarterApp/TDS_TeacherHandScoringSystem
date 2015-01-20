CREATE TABLE [dbo].[Logs] (
    [LogID]     INT            IDENTITY (1, 1) NOT NULL,
    [LogDate]   DATETIME       NOT NULL,
    [Category]  INT            NOT NULL,
    [Level]     INT            NOT NULL,
    [Message]   NVARCHAR (MAX) NOT NULL,
    [IpAddress] NVARCHAR (MAX) NULL,
    [Details]   NVARCHAR (MAX) NULL,
    CONSTRAINT [PK__Logs__5E5499A8173876EA] PRIMARY KEY CLUSTERED ([LogID] ASC)
);

