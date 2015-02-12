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

/*
	Description: DELETE	ALL ASSIGNMENTS THAT HAVE BEEN SCORED AND WHOSE SCORE HAS BEEN SUCCESSFULLY POSTED TO TIS.	
	Authors: Sai V. 
	Date Created: 1/9/2015

*/

CREATE PROCEDURE [dbo].[sp_RemoveAssignments] 
	@AssignmentList VARCHAR(MAX)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @StartDate  DATETIME
	DECLARE @EndDate	DATETIME	

	SET @StartDate = GETDATE()

	CREATE TABLE #AssignmentList(
		AssignmentID UNIQUEIDENTIFIER
	)	

	INSERT INTO #AssignmentList 
	SELECT items
	FROM dbo.fn_SplitDelimitedString(@AssignmentList, '|')
	
	
	CREATE INDEX ix_temp_AssignmentList ON #AssignmentList (AssignmentID)

	-- get list of responses that needs to be deleted
	SELECT a.ResponseID
	INTO #ResponseList
	FROM dbo.Assignments a
		JOIN #AssignmentList al ON al.AssignmentID = a.AssignmentID
	
	-- first, delete assignment data	
	DELETE a
	FROM dbo.Assignments a
		JOIN #AssignmentList al ON al.AssignmentID = a.AssignmentID

	-- now, delete response data					
	DELETE r
	FROM dbo.Responses r
		JOIN #ResponseList rl ON rl.ResponseID = r.ResponseID
	
	
	-- clean-up
	DROP TABLE #AssignmentList
	DROP TABLE #ResponseList
	
 	-- latency logging
	SET @EndDate = GETDATE()		  	
	EXEC dbo.sp_WritedbLatency 'dbo.sp_RemoveAssignments', @StartDate, @EndDate
	
END

