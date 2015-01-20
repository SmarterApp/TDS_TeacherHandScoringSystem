CREATE TABLE [dbo].[Responses] (
    [ResponseID]   UNIQUEIDENTIFIER NOT NULL,
    [BankKey]      INT              NOT NULL,
    [ContentLevel] VARCHAR (100)    NOT NULL,
    [Format]       VARCHAR (50)     NOT NULL,
    [ItemKey]      INT              NOT NULL,
    [Response]     NVARCHAR (MAX)   NULL,
    [ResponseDate] DATETIME         NULL,
    [SegmentId]    VARCHAR (255)    NOT NULL,
    CONSTRAINT [PK__Items__727E83EB50FB042B] PRIMARY KEY CLUSTERED ([ResponseID] ASC),
    CONSTRAINT [FK2EA47F61CF8ECC0] FOREIGN KEY ([BankKey], [ItemKey]) REFERENCES [dbo].[Items] ([BankKey], [ItemKey])
);


GO
CREATE STATISTICS [_dta_stat_1227151417_1_2_5]
    ON [dbo].[Responses]([ResponseID], [BankKey], [ItemKey]);

