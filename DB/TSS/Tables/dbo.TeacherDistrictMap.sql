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
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[TeacherDistrictMap](
	[DistrictID] [nvarchar](100) NOT NULL,
	[TeacherID] [varchar](250) NOT NULL,
 CONSTRAINT [TeacherDistrict_U] UNIQUE NONCLUSTERED 
(
	[DistrictID] ASC,
	[TeacherID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[TeacherDistrictMap]  WITH CHECK ADD  CONSTRAINT [FK_TeacherDistrictMap_Districts] FOREIGN KEY([DistrictID])
REFERENCES [dbo].[Districts] ([DistrictID])
GO

ALTER TABLE [dbo].[TeacherDistrictMap] CHECK CONSTRAINT [FK_TeacherDistrictMap_Districts]
GO

ALTER TABLE [dbo].[TeacherDistrictMap]  WITH CHECK ADD  CONSTRAINT [FK_TeacherDistrictMap_Teachers] FOREIGN KEY([TeacherID])
REFERENCES [dbo].[Teachers] ([TeacherID])
GO

ALTER TABLE [dbo].[TeacherDistrictMap] CHECK CONSTRAINT [FK_TeacherDistrictMap_Teachers]
GO



