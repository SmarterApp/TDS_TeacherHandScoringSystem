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
CREATE TABLE [dbo].[ConditionCodes] (
    [ConditionCodeId] INT            IDENTITY (1, 1) NOT NULL,
    [FullName]        NVARCHAR (MAX) NOT NULL,
    [ShortName]       NVARCHAR (MAX) NULL,
    [DimensionId]     INT            NULL,
    CONSTRAINT [PK__Conditio__A1AC31510F2D40CE] PRIMARY KEY CLUSTERED ([ConditionCodeId] ASC),
    CONSTRAINT [FK4D988D0F5ADE57E0] FOREIGN KEY ([DimensionId]) REFERENCES [dbo].[Dimensions] ([DimensionId])
);

