
CREATE PROCEDURE [dbo].[sp_SaveTest] 
    @TestId			VARCHAR(255)
  , @Name			NVARCHAR(MAX)
  , @Mode			NVARCHAR(MAX)
  , @Grade			NVARCHAR(MAX)	
  , @Subject		NVARCHAR(MAX)
  , @Version		NVARCHAR(MAX)
  , @AcademicYear	INT 
  , @AssessmentType NVARCHAR(MAX)
  , @Bank			INT
  , @Contract		NVARCHAR(MAX)
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
	
	IF NOT EXISTS (SELECT 1 FROM dbo.Tests WHERE TestID = @TestId)
	BEGIN
	BEGIN TRANSACTION	
		INSERT INTO [dbo].[Tests]
				   ([TestID]
				   ,[AcademicYear]
				   ,[AssessmentType]
				   ,[Bank]
				   ,[Contract]
				   ,[Grade]
				   ,[Mode]
				   ,[Name]
				   ,[Subject]
				   ,[Version])
			VALUES 
				 (  @TestId
				  , @AcademicYear
				  , @AssessmentType
				  , @Bank	
				  , @Contract
				  , @Grade
				  , @Mode			 
				  , @Name
				  , @Subject
				  , @Version)

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
	EXEC dbo.sp_WritedbLatency 'dbo.sp_SaveTest', @StartDate, @EndDate, @Comment
	           
END