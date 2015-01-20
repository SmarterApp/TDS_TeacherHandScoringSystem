
/*
	Description: RETURNS A  LIST OF ITEMS TO DISPLAYED ON THE ITEM LIST SCREEN
	Author: TGebicke - Summit
	DATE:12/18/2014
	
	Updated by: Sai - 1/5/2015 - Re-wrote code to optimize performance

*/

CREATE PROCEDURE [dbo].[sp_GetItemCount]
	@EmailList		NVARCHAR(MAX)
  , @TestFilter		NVARCHAR(500)
  ,	@SessionFilter	NVARCHAR(500)
  ,	@GradeFilter	NVARCHAR(500)
  ,	@SubjectFilter	NVARCHAR(500)
  ,	@ScorerFilter	NVARCHAR(500)
AS
BEGIN

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @StartDate  DATETIME
	DECLARE @EndDate	DATETIME	

	SET @StartDate = GETDATE()
	
	--CREATE TEMP Table
	CREATE TABLE #IdTable (EmailID VARCHAR(250))

	INSERT INTO #IdTable
	SELECT items
	FROM dbo.fn_SplitDelimitedString(@EmailList, '|')


	SELECT COUNT(*) 
	FROM dbo.Assignments a
		 JOIN dbo.Teachers te ON te.TeacherID = a.TeacherID
		 JOIN dbo.Tests t ON a.TestID = t.TestID
		 JOIN #IdTable e on e.EmailID = a.TeacherID 
	WHERE (@ScorerFilter = '' OR te.TeacherID = @ScorerFilter)
		  AND (@TestFilter = '' OR t.TestID = @TestFilter)
		  AND (@SessionFilter = '' OR a.SessionId = @SessionFilter)
		  AND (@GradeFilter = '' OR t.Grade = @GradeFilter)
		  AND (@SubjectFilter = '' OR  t.[Subject] = @SubjectFilter)
		  AND a.ScoreStatus != 2

	-- latency logging
	SET @EndDate = GETDATE()		  	
	EXEC dbo.sp_WritedbLatency 'dbo.sp_GetItemCount', @StartDate, @EndDate
		
END
