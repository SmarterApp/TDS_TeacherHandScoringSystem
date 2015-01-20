CREATE TABLE [dbo].[Dimensions] (
    [DimensionId] INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (MAX) NOT NULL,
    [BankKey]     INT            NOT NULL,
    [ItemKey]     INT            NOT NULL,
    [Min]         INT            NOT NULL,
    [Max]         INT            NOT NULL,
    CONSTRAINT [PK__Dimensio__1F7D4F110A688BB1] PRIMARY KEY CLUSTERED ([DimensionId] ASC),
    CONSTRAINT [FK_Bank_Item] FOREIGN KEY ([BankKey], [ItemKey]) REFERENCES [dbo].[Items] ([BankKey], [ItemKey])
);

