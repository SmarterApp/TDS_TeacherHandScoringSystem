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

