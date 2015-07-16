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
-- =============================================
-- Author:		Sai V.
-- Create date: 
-- Description:	
-- =============================================

CREATE PROCEDURE [dbo].[sp_ReAssignAssignments]
	@AssignmentList	VARCHAR(MAX)
  ,	@TeacherID		VARCHAR(250)
  , @TeacherName	VARCHAR(250)
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
		-- need to check if the input teacher exists in the table or not.
		-- if not, insert new row.
		IF NOT EXISTS (SELECT 1 FROM dbo.Teachers WHERE TeacherID = @TeacherID)
			INSERT INTO dbo.Teachers (TeacherID, Name) 
				VALUES (@TeacherID, @TeacherName)
		
		-- re-assign the assignment to new teacher	
		UPDATE a
		SET TeacherID = @TeacherID
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
	EXEC dbo.sp_WritedbLatency 'dbo.sp_ReAssignAssignments', @StartDate, @EndDate
			
END
