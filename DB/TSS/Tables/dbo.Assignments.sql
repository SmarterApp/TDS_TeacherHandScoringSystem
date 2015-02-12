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
CREATE TABLE [dbo].[Assignments] (
    [AssignmentID]   UNIQUEIDENTIFIER CONSTRAINT [DF__Assignmen__Assig__6501FCD8] DEFAULT (newid()) NOT NULL,
    [SessionId]      NVARCHAR (240)   NOT NULL,
    [OpportunityId]  BIGINT           NOT NULL,
    [OpportunityKey] UNIQUEIDENTIFIER NOT NULL,
    [ScoreStatus]    INT              NOT NULL,
    [ScoreData]      NVARCHAR (MAX)   NULL,
    [CallbackUrl]    NVARCHAR (MAX)   NULL,
    [ClientName]     VARCHAR (100)    NULL,
    [ResponseID]     UNIQUEIDENTIFIER NOT NULL,
    [TeacherID]      VARCHAR (250)    NULL,
    [SchoolID]       VARCHAR (100)    NULL,
    [StudentID]      BIGINT           NULL,
    [TestID]         VARCHAR (255)    NULL,
    [DateCreated]    SMALLDATETIME    CONSTRAINT [DF__Assignmen__DateC__0539C240] DEFAULT (getutcdate()) NOT NULL,
    CONSTRAINT [ix_pk_Assignments] PRIMARY KEY CLUSTERED ([AssignmentID] ASC),
    CONSTRAINT [FK_Assignments_Students] FOREIGN KEY ([StudentID]) REFERENCES [dbo].[Students] ([StudentID]),
    CONSTRAINT [FK_Assignments_Teachers] FOREIGN KEY ([TeacherID]) REFERENCES [dbo].[Teachers] ([TeacherID]),
    CONSTRAINT [fk_item] FOREIGN KEY ([ResponseID]) REFERENCES [dbo].[Responses] ([ResponseID]),
    CONSTRAINT [fk_school] FOREIGN KEY ([SchoolID]) REFERENCES [dbo].[Schools] ([SchoolID]),
    CONSTRAINT [fk_test] FOREIGN KEY ([TestID]) REFERENCES [dbo].[Tests] ([TestID])
);


GO
CREATE NONCLUSTERED INDEX [_ix_Assignments_TestIDTeachIDStatus]
    ON [dbo].[Assignments]([TestID] ASC, [TeacherID] ASC, [ScoreStatus] ASC)
    INCLUDE([AssignmentID], [ResponseID], [SchoolID], [SessionId], [StudentID]);


GO
CREATE NONCLUSTERED INDEX [ix_Assignments_TeachIDStatus]
    ON [dbo].[Assignments]([TeacherID] ASC, [ScoreStatus] ASC)
    INCLUDE([AssignmentID], [ResponseID], [SchoolID], [SessionId], [StudentID], [TestID]);


GO
CREATE STATISTICS [_dta_stat_1291151645_1_12]
    ON [dbo].[Assignments]([AssignmentID], [StudentID]);


GO
CREATE STATISTICS [_dta_stat_1291151645_9_12_13]
    ON [dbo].[Assignments]([ResponseID], [StudentID], [TestID]);


GO
CREATE STATISTICS [_dta_stat_1291151645_5_13_10]
    ON [dbo].[Assignments]([ScoreStatus], [TestID], [TeacherID]);


GO
CREATE STATISTICS [_dta_stat_1291151645_2_10]
    ON [dbo].[Assignments]([SessionId], [TeacherID]);


GO
CREATE STATISTICS [_dta_stat_1291151645_2_13]
    ON [dbo].[Assignments]([SessionId], [TestID]);


GO
CREATE STATISTICS [_dta_stat_1291151645_13_2]
    ON [dbo].[Assignments]([TestID], [SessionId]);


