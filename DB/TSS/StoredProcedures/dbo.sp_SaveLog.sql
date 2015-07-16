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
CREATE PROCEDURE sp_SaveLog 
            @LogDate datetime= NULL,
            @Category int,
            @Level int,
            @Message nvarchar(max),
            @IpAddress nvarchar(max),
            @Details nvarchar(max)
AS
BEGIN            

INSERT INTO [dbo].[Logs]
           ([LogDate]
           ,[Category]
           ,[Level]
           ,[Message]
           ,[IpAddress]
           ,[Details])
     VALUES(
           @LogDate, 
           @Category, 
           @Level, 
           @Message, 
           @IpAddress, 
           @Details)
          
END 

