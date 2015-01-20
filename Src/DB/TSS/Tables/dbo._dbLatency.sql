CREATE TABLE [dbo].[_dbLatency] (
    [ProcName]    VARCHAR (200)  NULL,
    [StartDate]   DATETIME       NULL,
    [EndDate]     DATETIME       NULL,
    [Duration_ms] INT            NULL,
    [Comments]    VARCHAR (8000) NULL
);


GO
CREATE CLUSTERED INDEX [ix_dbLatency_StartDate]
    ON [dbo].[_dbLatency]([StartDate] ASC);

