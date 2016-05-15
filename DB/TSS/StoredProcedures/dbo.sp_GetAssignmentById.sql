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

CREATE PROCEDURE [dbo].[sp_GetAssignmentById] 
	@AssignmentId UNIQUEIDENTIFIER
  , @PassPhrase  VARCHAR(100) = NULL	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @StartDate  DATETIME
	DECLARE @EndDate	DATETIME	

	SET @StartDate = GETDATE()
	
	SELECT a.AssignmentID
		 , SessionId
		 , OpportunityId
		 , OpportunityKey
		 , ScoreData
		 , ScoreStatus
		 , CallbackUrl
		 , ClientName
		 , TeacherID
		 , a.TestID
		 , r.Response
		 , t.Name			AS TestName
		 , st.FirstName + ' ' + st.LastName AS StudentName		 
		 , i.ItemKey
		 , i.BankKey
		 , i.HandScored
		 , i.Passage
	FROM dbo.Assignments a
		INNER JOIN dbo.Responses r 
		ON a.ResponseID = r.ResponseID 
		INNER JOIN dbo.Items i 
		ON i.BankKey = r.BankKey AND i.ItemKey = r.ItemKey
		INNER JOIN dbo.Tests t
		ON a.TestID = t.TestID
		INNER JOIN dbo.Students st
		ON a.StudentID = st.StudentID
	WHERE a.AssignmentID = @AssignmentId
  
 
 	-- latency logging
	SET @EndDate = GETDATE()		  	
	EXEC dbo.sp_WritedbLatency 'dbo.sp_GetAssignmentById', @StartDate, @EndDate
	
END
