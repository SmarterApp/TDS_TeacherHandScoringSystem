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

