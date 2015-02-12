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
CREATE TABLE [dbo].[Tests] (
    [TestID]         VARCHAR (255) NOT NULL,
    [AcademicYear]   INT           NOT NULL,
    [AssessmentType] VARCHAR (50)  NOT NULL,
    [Bank]           INT           NOT NULL,
    [Contract]       VARCHAR (100) NOT NULL,
    [Grade]          VARCHAR (50)  NOT NULL,
    [Mode]           VARCHAR (25)  NOT NULL,
    [Name]           VARCHAR (255) NOT NULL,
    [Subject]        VARCHAR (100) NOT NULL,
    [Version]        VARCHAR (25)  NOT NULL,
    CONSTRAINT [PK__Tests__8CC331002BFE89A6] PRIMARY KEY CLUSTERED ([TestID] ASC)
);


