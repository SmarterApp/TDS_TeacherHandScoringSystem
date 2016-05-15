
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_SaveActivityLog] 
    @LogDate datetime= NULL,
    @Category int,
    @Level int,
    @Message nvarchar(max),
    @IpAddress nvarchar(max),
    @Details nvarchar(max)
AS
BEGIN            

	INSERT INTO [dbo].[ActivityLog]
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


