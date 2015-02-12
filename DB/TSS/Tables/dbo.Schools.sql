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
CREATE TABLE [dbo].[Schools] (
    [SchoolID]   VARCHAR (100)  NOT NULL,
    [SchoolName] NVARCHAR (500) NOT NULL,
    [StateName]  NVARCHAR (500) NOT NULL,
    [DistrictID] NVARCHAR (100) NULL,
    CONSTRAINT [PK__Schools__3DA4677B123EB7A3] PRIMARY KEY CLUSTERED ([SchoolID] ASC),
    CONSTRAINT [FK_Districts] FOREIGN KEY ([DistrictID]) REFERENCES [dbo].[Districts] ([DistrictID])
);


