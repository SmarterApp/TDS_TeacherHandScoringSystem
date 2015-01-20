
/*
	Description: RETURNS A  LIST OF ITEMS TO DISPLAYED ON THE ITEM LIST SCREEN
	Author: TGebicke - Summit
	DATE:12/18/2014
	
	Updated by: Sai - 1/5/2015 - Re-wrote code to optimize performance

*/

CREATE PROCEDURE [dbo].[sp_GetAssignedItems]
	@TeacherId		VARCHAR(250)
  ,	@TestFilter		VARCHAR(255)
  ,	@SessionFilter	NVARCHAR(500)
  ,	@GradeFilter	VARCHAR(50)
  ,	@SubjectFilter	VARCHAR(100)
  , @ScorerFilter	VARCHAR(100)
AS
BEGIN

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @StartDate			DATETIME
	DECLARE @EndDate			DATETIME	
	
	SET @StartDate = GETDATE()

	-- Declare variables to set/optimize the filter conditions
	DECLARE @TestFilterCond		VARCHAR(500)
	DECLARE @SessionFilterCond	VARCHAR(500)
	DECLARE @GradeFilterCond	VARCHAR(500)
	DECLARE @SubjectFilterCond	VARCHAR(500)
	

	SET @TestFilterCond		= (CASE WHEN @TestFilter = '' THEN '1 = 1' ELSE 'a.TestID = @TestFilter' END)
	SET @SessionFilterCond  = (CASE WHEN @SessionFilter = '' THEN '1 = 1' ELSE 'a.SessionId = @SessionFilter' END)
	SET @GradeFilterCond	= (CASE WHEN @GradeFilter = '' THEN '1 = 1' ELSE 't.Grade = @GradeFilter' END)
	SET @SubjectFilterCond  = (CASE WHEN @SubjectFilter = '' THEN '1 = 1' ELSE 't.subject = @SubjectFilter' END)


	DECLARE @SQL_2 NVARCHAR(4000)
	SET @SQL_2 = ''
	
	-- we need to join to Tests table only when we have to filter by grade or subject	
	IF @GradeFilter <> '' OR @SubjectFilter <> ''
		SET @SQL_2 = 'JOIN dbo.Tests t ON a.TestID = t.TestID'

		
	DECLARE @SQL_1 NVARCHAR(4000)
	SET @SQL_1 = ' SELECT a.AssignmentId	 
					FROM dbo.Assignments a
					' + @SQL_2 + '
					WHERE a.TeacherID = @TeacherID 
						  AND ' + @TestFilterCond + '
						  AND ' + @SessionFilterCond + '
						  AND ' + @GradeFilterCond + '
						  AND ' + @SubjectFilterCond + '
						  AND a.ScoreStatus IN (0, 1)
					ORDER BY a.AssignmentId'

	--PRINT @SQL_1
							  
	DECLARE @ParamsList_1 NVARCHAR(1000) = N'@TeacherId VARCHAR(250), @TestFilter VARCHAR(255), @SessionFilter NVARCHAR(500), @GradeFilter VARCHAR(50), @SubjectFilter VARCHAR(100)'
	EXECUTE sp_executesql @SQL_1
	   , @ParamsList_1
	   , @TeacherId 	   
	   , @TestFilter
	   , @SessionFilter	
	   , @GradeFilter
	   , @SubjectFilter	

	
	-- latency logging
	SET @EndDate = GETDATE()		  	
	EXEC dbo.sp_WritedbLatency 'dbo.sp_GetAssignedItems', @StartDate, @EndDate

END
