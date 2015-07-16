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
CREATE TABLE [dbo].[Logs] (
    [LogID]     INT            IDENTITY (1, 1) NOT NULL,
    [LogDate]   DATETIME       NOT NULL,
    [Category]  INT            NOT NULL,
    [Level]     INT            NOT NULL,
    [Message]   NVARCHAR (MAX) NOT NULL,
    [IpAddress] NVARCHAR (MAX) NULL,
    [Details]   NVARCHAR (MAX) NULL,
    CONSTRAINT [PK__Logs__5E5499A8173876EA] PRIMARY KEY CLUSTERED ([LogID] ASC)
);

