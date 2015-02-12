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

CREATE PROCEDURE [dbo].[sp_GetAssignmentsByIDList]
	@AssignmentList	VARCHAR(MAX)
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


	SELECT a.AssignmentID
		 , a.CallbackUrl
		 , a.ClientName
		 , a.ScoreStatus
		 , a.ScoreData
		 , a.OpportunityKey
		 , r.BankKey
		 , r.ItemKey
		 , r.Format
		 , a.TeacherID
	FROM dbo.Assignments a
		 JOIN #AssignmentList al ON al.AssignmentID = a.AssignmentID
		 JOIN dbo.Responses r ON r.ResponseID = a.ResponseID
   WHERE a.ScoreStatus = 1
	
	-- clean-up
	DROP TABLE #AssignmentList

	-- latency logging
	SET @EndDate = GETDATE()		  	
	EXEC dbo.sp_WritedbLatency 'dbo.sp_GetAssignmentsByIDList', @StartDate, @EndDate
			
END

