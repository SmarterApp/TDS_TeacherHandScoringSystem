CREATE TABLE [dbo].[Students] (
    [StudentID]  BIGINT         NOT NULL,
    [Dob]        DATETIME       NOT NULL,
    [FirstName]  NVARCHAR (MAX) NULL,
    [LastName]   NVARCHAR (MAX) NULL,
    [SSID]       NVARCHAR (MAX) NULL,
    [Grade]      NVARCHAR (MAX) NULL,
    [Name]       NVARCHAR (MAX) NULL,
    [TDSLoginId] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK__Students__32C52A7907C12930] PRIMARY KEY CLUSTERED ([StudentID] ASC)
);

