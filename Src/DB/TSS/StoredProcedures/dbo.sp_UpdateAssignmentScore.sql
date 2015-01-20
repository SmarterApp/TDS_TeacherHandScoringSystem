-- =============================================
-- Author:		Sai V.
-- Create date: 
-- Description:	
-- =============================================

CREATE PROCEDURE [dbo].[sp_UpdateAssignmentScore]
	@AssignmentId	UNIQUEIDENTIFIER
  , @ScoreData		VARCHAR(MAX)	
  ,	@ScoreStatus	INT
AS
BEGIN

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @StartDate  DATETIME
	DECLARE @EndDate	DATETIME	

	SET @StartDate = GETDATE()	


	-- update assignment score status and score data
	UPDATE a
	SET ScoreStatus = @ScoreStatus
	  , ScoreData	= @ScoreData
	FROM dbo.Assignments a
	WHERE AssignmentID = @AssignmentId		

	
	-- latency logging
	SET @EndDate = GETDATE()		  	
	EXEC dbo.sp_WritedbLatency 'dbo.sp_UpdateAssignmentScore', @StartDate, @EndDate
			
END
