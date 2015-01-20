

CREATE PROCEDURE [dbo].[sp_SaveTeacher] 
    @TeacherId		VARCHAR(100)
  , @TeacherName	VARCHAR(500)	  
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
	
	IF NOT EXISTS (SELECT 1 FROM dbo.Teachers WHERE TeacherID = @TeacherId)
	BEGIN
	BEGIN TRANSACTION	
		INSERT INTO [dbo].[Teachers]
			   ( [TeacherID]
			   , [Name])
		 VALUES
			   ( @TeacherID
			   , @TeacherName
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
	EXEC dbo.sp_WritedbLatency 'dbo.sp_SaveTeacher', @StartDate, @EndDate, @Comment
	           
END


