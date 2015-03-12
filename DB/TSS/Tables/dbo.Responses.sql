/*
 /*******************************************************************************                                                                                                                                    
  * Educational Online Test Delivery System                                                                                                                                                                       
  * Copyright (c) 2014 American Institutes for Research                                                                                                                                                              
  *                                                                                                                                                                                                                  
  * Distributed under the AIR Open Source License, Version 1.0                                                                                                                                                       
  * See accompanying file AIR-License-1_0.txt or at                                                                                                                                                                  
  * http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf                                                                                                                                                 
  ******************************************************************************/ 
*/
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
/****** Object:  Index [BankKeyAndItemKeyComp]    Script Date: 03/10/2015 13:56:05 ******/
CREATE NONCLUSTERED INDEX [ix_BankKeyAndItemKeyComp] ON [dbo].[Responses] 
(
	[BankKey] ASC,
	[ItemKey] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

GO
CREATE STATISTICS [_dta_stat_1227151417_1_2_5]
    ON [dbo].[Responses]([ResponseID], [BankKey], [ItemKey]);


