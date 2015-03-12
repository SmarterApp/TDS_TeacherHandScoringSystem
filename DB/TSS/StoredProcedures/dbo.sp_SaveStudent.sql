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


CREATE PROCEDURE [dbo].[sp_SaveStudent] 
    @StudentId		BIGINT
  , @DOB			DATETIME = NULL
  , @FirstName		VARCHAR(500)    
  , @LastName		VARCHAR(500)  
  , @SSID			VARCHAR(500)
  , @Name			NVARCHAR(MAX) 
  , @TdsLoginId		NVARCHAR(MAX)
  , @Grade			VARCHAR(500)	  
AS
BEGIN 

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @StartDate  DATETIME
	DECLARE @EndDate	DATETIME
	DECLARE @Comment	VARCHAR(8000)	

	SET @StartDate = GETDATE()

	DECLARE @ErrorFlag	BIT
	DECLARE @Err		VARCHAR(8000)
	SET @ErrorFlag = 0

	IF NOT EXISTS (SELECT 1 FROM dbo.Students WHERE StudentID = @StudentId)	
	BEGIN	
	BEGIN TRANSACTION
	BEGIN TRY	
		INSERT INTO [dbo].[Students]
				   ([StudentID]
				   ,[Dob]
				   ,[FirstName]
				   ,[LastName]
				   ,[SSID]
				   ,[Grade]
				   ,[Name]
				   ,[TDSLoginId])
			 VALUES
				   ( @StudentID
				   , @Dob
				   , @FirstName
				   , @LastName
				   , @SSID
				   , @Grade
				   , @Name
				   , @TDSLoginId
				)
	END TRY
	BEGIN CATCH
		ROLLBACK
		SET @Err = ERROR_MESSAGE()
		SET @ErrorFlag = 1
		EXEC dbo.sp_WritedbLatency 'dbo.sp_SaveStudent', @StartDate, @EndDate, @Err
		RETURN
	END CATCH			
	COMMIT TRANSACTION
	END
	ELSE BEGIN 
	    IF ((@FirstName IS NOT NULL AND @LastName IS NOT NULL) OR (@Name IS NOT NULL))
	    BEGIN 
			UPDATE [dbo].[Students]
				  SET  
				  Dob=@DOB,
				  FirstName=@FirstName,
				  LastName=@LastName,
				  SSID=@SSID,
				  Grade=@Grade,
				  Name=@Name,
				  TDSLoginId = @TDSLoginId
				WHERE StudentID=@StudentID
		END 
	END
	SELECT @ErrorFlag -- 0 indicates success; 1 indicates failure

	-- latency logging
	SET @EndDate = GETDATE()
	EXEC dbo.sp_WritedbLatency 'dbo.sp_SaveStudent', @StartDate, @EndDate, @Comment
		
END

