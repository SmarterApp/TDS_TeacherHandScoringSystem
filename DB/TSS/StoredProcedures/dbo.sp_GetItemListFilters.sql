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
	Description: RETURNS A SINGLE LIST OF DISTINCT VALUES FOR EACH FILTER
	EXAMPLE: sp_GetItemListFilters @emailist = 'ownitemscorer01@example.com|ownitemscorer01@example.com'
	
	Updated by: Sai - 1/5/2015 - Re-wrote code to optimize performance

*/

CREATE PROCEDURE [dbo].[sp_GetItemListFilters] 
	@TeacherIDList VARCHAR(4000)
AS
BEGIN 

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @StartDate  DATETIME
	DECLARE @EndDate	DATETIME	

	SET @StartDate = GETDATE()
	
	CREATE TABLE #TeacherList (
		TeacherID VARCHAR(250)
	)

	INSERT INTO #TeacherList
	SELECT items 
	FROM dbo.fn_SplitDelimitedString(@TeacherIDList,'|')	


	SELECT t.Grade,t.[Subject], t.TestID, t.Name TestName, st.sessionid  SessionId, st.Name AssignedTo, st.TeacherID
	FROM dbo.Tests t 
		JOIN 
			( SELECT DISTINCT a.SessionID, a.TestID, te.Name, te.TeacherID
			  FROM dbo.Assignments a
				JOIN dbo.Teachers te ON a.TeacherId = te.TeacherID
				JOIN #TeacherList tl ON tl.TeacherID = a.TeacherID
			  WHERE a.ScoreStatus in (0, 1)
			) st 
		ON t.TestID = st.testid 
	
		
	-- clean-up
	DROP TABLE  #TeacherList


	-- latency logging
	SET @EndDate = GETDATE()		  	
	EXEC dbo.sp_WritedbLatency 'dbo.sp_GetItemListFilters', @StartDate, @EndDate
		
END



