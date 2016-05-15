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

GO

/* Aaron  Added oppkey index   */
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Assignments](
	[AssignmentID] [uniqueidentifier] NOT NULL,
	[SessionId] [nvarchar](240) NOT NULL,
	[OpportunityId] [bigint] NOT NULL,
	[OpportunityKey] [uniqueidentifier] NOT NULL,
	[ScoreStatus] [int] NOT NULL,
	[ScoreData] [nvarchar](max) NULL,
	[CallbackUrl] [nvarchar](max) NULL,
	[ClientName] [varchar](100) NULL,
	[ResponseID] [uniqueidentifier] NOT NULL,
	[TeacherID] [varchar](250) NULL,
	[StudentID] [bigint] NULL,
	[TestID] [varchar](255) NULL,
	[DateCreated] [smalldatetime] NOT NULL,
 CONSTRAINT [ix_pk_Assignments] PRIMARY KEY CLUSTERED 
(
	[AssignmentID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
CREATE NONCLUSTERED INDEX [assignment_opkey_index] ON [dbo].[Assignments] 
(
	[OpportunityKey] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

-- Required to avoid deadlock when inserting assignments.
CREATE NONCLUSTERED INDEX [assignment_responseid_index] ON [dbo].[Assignments] 
(
	[ResponseID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

drop index [assignment_responseid_index] on assignments
SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[Assignments]  WITH NOCHECK ADD  CONSTRAINT [FK_Assignments_Students] FOREIGN KEY([StudentID])
REFERENCES [dbo].[Students] ([StudentID])
GO

ALTER TABLE [dbo].[Assignments] CHECK CONSTRAINT [FK_Assignments_Students]
GO

ALTER TABLE [dbo].[Assignments]  WITH NOCHECK ADD  CONSTRAINT [FK_Assignments_Teachers] FOREIGN KEY([TeacherID])
REFERENCES [dbo].[Teachers] ([TeacherID])
GO

ALTER TABLE [dbo].[Assignments] CHECK CONSTRAINT [FK_Assignments_Teachers]
GO

ALTER TABLE [dbo].[Assignments]  WITH NOCHECK ADD  CONSTRAINT [fk_item] FOREIGN KEY([ResponseID])
REFERENCES [dbo].[Responses] ([ResponseID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Assignments] CHECK CONSTRAINT [fk_item]
GO

ALTER TABLE [dbo].[Assignments]  WITH NOCHECK ADD  CONSTRAINT [fk_test] FOREIGN KEY([TestID])
REFERENCES [dbo].[Tests] ([TestID])
GO

ALTER TABLE [dbo].[Assignments] CHECK CONSTRAINT [fk_test]
GO

ALTER TABLE [dbo].[Assignments] ADD  CONSTRAINT [DF__Assignmen__Assig]  DEFAULT (newsequentialid()) FOR [AssignmentID]
GO

ALTER TABLE [dbo].[Assignments] ADD  CONSTRAINT [DF__Assignmen__DateC__0539C240]  DEFAULT (getutcdate()) FOR [DateCreated]
GO


