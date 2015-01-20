CREATE TABLE [dbo].[Schools] (
    [SchoolID]   VARCHAR (100)  NOT NULL,
    [SchoolName] NVARCHAR (500) NOT NULL,
    [StateName]  NVARCHAR (500) NOT NULL,
    [DistrictID] NVARCHAR (100) NULL,
    CONSTRAINT [PK__Schools__3DA4677B123EB7A3] PRIMARY KEY CLUSTERED ([SchoolID] ASC),
    CONSTRAINT [FK_Districts] FOREIGN KEY ([DistrictID]) REFERENCES [dbo].[Districts] ([DistrictID])
);

