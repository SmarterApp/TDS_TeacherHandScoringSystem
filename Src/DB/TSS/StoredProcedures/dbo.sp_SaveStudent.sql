
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

	DECLARE @ErrorFlag BIT
	SET @ErrorFlag = 0

	IF NOT EXISTS (SELECT 1 FROM dbo.Students WHERE StudentID = @StudentId)	
	BEGIN	
	BEGIN TRANSACTION	
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
	COMMIT TRANSACTION
	END
	
	IF @@TRANCOUNT > 0
	BEGIN
		ROLLBACK TRANSACTION
		SET @ErrorFlag = 1
	END	

	SELECT @ErrorFlag -- 0 indicates success; 1 indicates failure

	-- latency logging
	SET @EndDate = GETDATE()	
	SET @Comment = '@ErrorFlag:' + (CASE @ErrorFlag WHEN 0 THEN 'Success' ELSE 'Failure' END)
	EXEC dbo.sp_WritedbLatency 'dbo.sp_SaveStudent', @StartDate, @EndDate, @Comment
		
END