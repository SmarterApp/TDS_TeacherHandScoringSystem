CREATE TABLE [dbo].[Items] (
    [BankKey]          INT            NOT NULL,
    [ExemplarURL]      NVARCHAR (MAX) NULL,
    [ItemKey]          INT            NOT NULL,
    [Subject]          NVARCHAR (MAX) NULL,
    [Grade]            NVARCHAR (MAX) NULL,
    [Description]      NVARCHAR (MAX) NULL,
    [RubricListXML]    NVARCHAR (MAX) NULL,
    [TrainingGuideURL] NVARCHAR (MAX) NULL,
    [Layout]           NVARCHAR (MAX) NULL,
    [Passage]          INT            NULL,
    [HandScored]       BIT            NULL,
    CONSTRAINT [PK_Items] PRIMARY KEY CLUSTERED ([BankKey] ASC, [ItemKey] ASC)
);

