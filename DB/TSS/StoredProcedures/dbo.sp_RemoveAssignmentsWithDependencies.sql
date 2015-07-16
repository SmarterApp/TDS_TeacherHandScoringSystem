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
GO
/****** Object:  StoredProcedure [dbo].[sp_RemoveAssignmentsWithDependencies]    Script Date: 03/23/2015 11:22:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
	Description: DELETE	ALL ASSIGNMENTS THAT HAVE BEEN SCORED AND WHOSE SCORE HAS BEEN SUCCESSFULLY POSTED TO TIS.	
	   also, delete some dependent items that are used in scoring items, if any
	Authors: Aaron  (based on Sai V. code)
	Date Created: 3/2/2015

*/

CREATE PROCEDURE [dbo].[sp_RemoveAssignmentsWithDependencies] 
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

	-- So that we can delete any dependent, scored items, save information about those assignments
	-- with the same opportunity
	DECLARE @opps TABLE (OpportunityKey UNIQUEIDENTIFIER)
	
	--load oppkeys from assignments to to be deleted
	INSERT INTO @opps 
	SELECT DISTINCT assign.OpportunityKey
	FROM #AssignmentList list 
	JOIN Assignments assign  
	ON assign.AssignmentID=list.AssignmentID
	
		
	CREATE TABLE #ScoredOpps(OpportunityKey UNIQUEIDENTIFIER)	
	INSERT INTO #ScoredOpps 
	SELECT DISTINCT list.OpportunityKey 
	FROM @opps list 
	JOIN Assignments assign 
	on assign.OpportunityKey=list.OpportunityKey and  assign.ScoreStatus=2  
	
	
	CREATE TABLE #NotToDelete (OpportunityKey UNIQUEIDENTIFIER)	
	-- Now we look for assignments that are scored and share these opportunity ids.
	-- Find all opportunities that have any non-scored assignments left.
	-- leave those alone.
	INSERT INTO #NotToDelete 
	SELECT DISTINCT assign.OpportunityKey 
	FROM assignments assign
	JOIN @opps opps 
	ON opps.OpportunityKey=assign.OpportunityKey
	WHERE assign.ScoreStatus <> 2 
	GROUP BY assign.OpportunityKey
	
		
	DELETE dep 
	FROM #ScoredOpps dep
	JOIN #NotToDelete nodel 
	ON nodel.OpportunityKey=dep.OpportunityKey

    DECLARE @ErrorFlag  BIT	
	SET @ErrorFlag = 0
	
    BEGIN TRANSACTION
	BEGIN TRY	
	
	-- first deleted assignments marked as completed
	DELETE a
	FROM dbo.Assignments a
	    JOIN #AssignmentList assign ON a.AssignmentID = assign.AssignmentID
	-- Any left overs have no dependencies, so we can delete them
	DELETE a
	FROM dbo.Assignments a	    
		JOIN #ScoredOpps depend ON depend.OpportunityKey= a.OpportunityKey
		
	END TRY
	BEGIN CATCH
		ROLLBACK
		DECLARE @Err NVARCHAR(Max)
		SET @Err = ERROR_MESSAGE()
		SET @ErrorFlag = 1
		EXEC dbo.sp_WritedbLatency 'dbo.sp_RemoveAssignmentsWithDependencies', @StartDate, @EndDate, @Err
		RETURN
	END CATCH			
	COMMIT TRANSACTION
		
	SELECT @ErrorFlag -- 0 indicates success; 1 indicates failure	
	-- clean-up	
	DROP TABLE #NotToDelete	
	DROP TABLE #AssignmentList
	DROP TABLE #ScoredOpps
	
 	-- latency logging
	SET @EndDate = GETDATE()		  	
	EXEC dbo.sp_WritedbLatency 'dbo.sp_RemoveAssignmentsWithDependencies', @StartDate, @EndDate
	
END
