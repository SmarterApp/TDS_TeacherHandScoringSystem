-- =============================================
-- Author:		Sai V.
-- Create date: 
-- Description:	
-- =============================================

CREATE PROCEDURE [dbo].[sp_UpdateAssignmentsStatus]
	@AssignmentList	VARCHAR(MAX)
  ,	@Status			INT
AS
BEGIN

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @StartDate  DATETIME
	DECLARE @EndDate	DATETIME	

	SET @StartDate = GETDATE()	

	DECLARE @ErrorFlag BIT
	SET @ErrorFlag = 0

	CREATE TABLE #AssignmentList(
		AssignmentID UNIQUEIDENTIFIER
	)	

	INSERT INTO #AssignmentList 
	SELECT items
	FROM dbo.fn_SplitDelimitedString(@AssignmentList, '|')

	BEGIN TRANSACTION			
		-- update assignment status
		UPDATE a
		SET ScoreStatus = @Status
		FROM dbo.Assignments a
			 JOIN #AssignmentList al ON al.AssignmentID = a.AssignmentID
	COMMIT TRANSACTION
	
	IF @@TRANCOUNT > 0
	BEGIN
		ROLLBACK TRANSACTION
		SET @ErrorFlag = 1
	END	

	SELECT @ErrorFlag -- 0 indicates success; 1 indicates failure
	
	-- clean-up
	DROP TABLE #AssignmentList

	-- latency logging
	SET @EndDate = GETDATE()		  	
	EXEC dbo.sp_WritedbLatency 'dbo.sp_UpdateAssignmentsStatus', @StartDate, @EndDate
			
END
