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

