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
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
	Description: SAVES ASSIGNMENT AND RESPONSE DATA TO THE APPROPRIATE TABLES
	Author: Sai V.
	DATE: 1/5/2015

	NotScored = 0
    TentativeScore = 1
	Scored = 2
	
	DATE: 3/6/2015 - Aaron added dependent items handling
	3/17/15 - Added some refactoring in deadlock investigation.  Cover whole thing in transaction
  	
*/
            
CREATE PROCEDURE [dbo].[sp_SaveAssignmentAndResponses]
    @xmlInputs	 XML   	  
AS
BEGIN 

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @StartDate  DATETIME
	DECLARE @EndDate	DATETIME
	
	SET @StartDate = GETDATE()
	
	DECLARE @RespFlag	BIT	
	DECLARE @hDoc		INT
	DECLARE @ErrorFlag	BIT
	DECLARE @Err		VARCHAR(8000)
	
	SET @ErrorFlag = 0

	EXEC sp_xml_preparedocument @hDoc OUTPUT, @xmlInputs

	-- extract data from XML
    SELECT *
    INTO #Assignment
    FROM OPENXML (@hDoc, '/Root/Assignment') 
	WITH ( TestId			VARCHAR(255)
		 , TeacherId		NVARCHAR(250) 
		 , StudentId		BIGINT	
		 , SessionId		NVARCHAR(240)
		 , OpportunityId	BIGINT
		 , OpportunityKey	UNIQUEIDENTIFIER
		 , ClientName		VARCHAR(100)
		 , CallbackUrl		NVARCHAR(MAX)) AS x

    SELECT *
		 , NEWID() AS ResponseID 
    INTO #ResponseList
    FROM OPENXML (@hDoc, '/Root/ItemList/Item') 
	WITH ( ItemKey		INT				
		 , BankKey		INT
		 , ContentLevel VARCHAR(100)
		 , Format		VARCHAR(50)
		 , SegmentId	VARCHAR(255)	
		 , ScoreStatus	INT
		 , ResponseDate DATETIME
		 , Response		NVARCHAR(MAX) 'Response/.') AS x

    

    declare @oppKey  UNIQUEIDENTIFIER
    set @oppKey = (select top 1 OpportunityKey from #Assignment)
	
	declare @dependentItems TABLE (
	       ItemKey		INT				
		 , BankKey		INT
		 )
    
    -- Get the machine-scored items from response list and move them to another table
    -- This is the list of dependent items
    insert into @dependentItems select 
          resp.ItemKey
        , resp.BankKey
        from #ResponseList resp
    join dbo.Items itm (NOLOCK) on itm.BankKey=resp.BankKey and itm.ItemKey=resp.ItemKey and itm.HandScored = 0
    
    -- Force machine-scored items to be scored so teachers can't score them
    Update resp SET ScoreStatus=2 from #ResponseList resp 
    join @dependentItems dep on dep.BankKey=resp.BankKey and dep.ItemKey=resp.ItemKey    
    
	-- check if the response data is already loaded
	-- set the flag to 0 if rows does not exists, 1 if row exists
	  SET @RespFlag = (CASE WHEN exists (SELECT 1 from Assignments (NOLOCK) where OpportunityKey=@oppKey)
							THEN 1
							ELSE 0 END)											   
							

	-- USE CASE#1: check if data already exists in the database, and if the scored status is set to 'Scored' for all responses in the XML
	-- if conditions satisfy, do nothing
	IF @RespFlag = 1 AND NOT EXISTS (SELECT 1 FROM #ResponseList WHERE ScoreStatus != 2)
	BEGIN
		--PRINT 'USE CASE#1'
		SET @EndDate = GETDATE()	
		EXEC dbo.sp_WritedbLatency 'dbo.sp_SaveAssignmentAndResponses', @StartDate, @EndDate, 'SP exited conditionally. Response data already exists.'
		RETURN		
	END	
    
    -- insert into dbo.AaronXmlDbg select @xmlInputs (uncomment for debugging)

	-- USE CASE#2: check if data already exists in the database
	-- @RespFlag = 0 indicates record does not exist. Insert data into the appropriate tables
	BEGIN TRANSACTION	
	BEGIN TRY
	
	--PRINT 'USE CASE#2'
	-- IF @RespFlag = 1
	-- #responseList and #assignmentList already contain the correct values, so we defer the 
	-- commit to the last step to avoid deadlock.
	IF @RespFlag = 1
	BEGIN
		
	-- USE CASE#3: check if data already exists in the database and the scoreStatus for each of the responses	
	-- If scoreStatus for a given response is 'Scored' => update response and scoreStatus data
	-- If scoreStatus for a given response is NOT 'Scored' => update only response data
	-- BEGIN
		--PRINT 'USE CASE#3'
		
		DECLARE @UpdateToResponseData TABLE (
			AssignmentID		UNIQUEIDENTIFIER
		  , ResponseID			UNIQUEIDENTIFIER
		  , ScoreStatus			INT
		  , New_ScoreStatus		INT
		  , New_Response		VARCHAR(MAX)
		  , New_ResponseDate	DATETIME 
		  , HasRespChanged		BIT
		  , MachineScored        BIT
		)
		
		INSERT INTO @UpdateToResponseData		
		SELECT a.AssignmentID
			 , a.ResponseID
			 , a.ScoreStatus
			 , rl.ScoreStatus   AS New_ScoreStatus
			 , rl.Response		AS New_Response 
			 , rl.ResponseDate	AS New_ResponseDate	 
			 , (CASE WHEN rl.Response = r.Response THEN 0 ELSE 1 END) HasRespChanged
			 , (CASE WHEN EXISTS (SELECT 1 from @dependentItems dep
			      where dep.ItemKey = r.ItemKey and dep.BankKey = r.BankKey) THEN 1 ELSE 0 END) MachineScored
		FROM dbo.Assignments a (NOLOCK)
			JOIN dbo.Responses r (NOLOCK) ON r.ResponseID = a.ResponseID
			JOIN #ResponseList rl ON rl.ItemKey = r.ItemKey AND rl.BankKey = r.BankKey AND rl.SegmentID = r.SegmentID
			where a.OpportunityKey=@oppKey
		-- Aaron 3/6/2015.  Consider scored items also since we have dependent items. 
		-- WHERE rl.ScoreStatus IN  (0, 1) --Ignore responses that have scoreStatus = 'Scored' on XML		
			 
		-- update response if it has changed since last entry		
		UPDATE r
		SET Response = u.New_Response
		  , ResponseDate = u.New_ResponseDate
		FROM dbo.Responses r 
			JOIN @UpdateToResponseData u ON u.ResponseID = r.ResponseID
		WHERE u.HasRespChanged = 1	
			
		-- if response has changed and score status is 'Scored', reset the score status
		-- But only handscored items, though
		UPDATE a
		SET ScoreStatus = 0 --NOT SCORED
		  , ScoreData   = NULL 
		FROM dbo.Assignments a 
			JOIN @UpdateToResponseData u ON u.AssignmentID = a.AssignmentID
		WHERE a.ScoreStatus = 2  --SCORED
			AND u.HasRespChanged = 1 and u.MachineScored = 0
			
		-- non-HS items MUST be set to scored, so we don't score them.
		-- This will fix existing records where this was not done.
		UPDATE a
		SET ScoreStatus = 2 --NOT SCORED
		FROM dbo.Assignments a
			JOIN @UpdateToResponseData u ON u.AssignmentID = a.AssignmentID
		WHERE u.MachineScored = 1
		
		declare @oppsToDelete table (assignmentID uniqueidentifier,responseID uniqueidentifier)
		
        -- If we are importing dependent items that were not imported on a previous run, 
        -- there may be some responses that are not in the table yet.  Otherwise the 
        -- logic above added them and we can forget about them
        delete resp from #responseList rlist 
        JOIN Assignments assign (NOLOCK) ON assign.OpportunityKey=@oppKey
        join Responses resp (NOLOCK) on resp.ResponseID=assign.ResponseID
        where resp.BankKey = rlist.BankKey and resp.ItemKey=rlist.ItemKey                
        				
    END   -- If respFlag == 1     				
    
    -- We have updated any records, and what is left are adds.  Insert response first to get responseID
	INSERT INTO [dbo].[Responses]
			   ([ResponseID]
			   ,[BankKey]
			   ,[ContentLevel]
			   ,[Format]
			   ,[ItemKey]
			   ,[Response]
			   ,[ResponseDate]
			   ,[SegmentId])
		SELECT ResponseID
			 , BankKey
			 , ContentLevel
			 , Format
			 , ItemKey
			 , Response
			 , ResponseDate
			 , SegmentId
		FROM #ResponseList 
		
				
	INSERT INTO [dbo].[Assignments]
			   ([SessionId]
			   ,[OpportunityId]
			   ,[OpportunityKey]
			   ,[ScoreStatus]
			   ,[CallbackUrl]
			   ,[ClientName]
			   ,[ResponseID]
			   ,[TeacherID]		
			   ,[StudentID]
			   ,[TestID])
		SELECT SessionId
			 , OpportunityId
			 , OpportunityKey
			 , ScoreStatus
			 , CallbackUrl
			 , ClientName
			 , r.ResponseID
			 , TeacherID			
			 , StudentID
			 , TestID
		FROM #Assignment a, #ResponseList r						
			
	-- clean-up
	DROP TABLE #Assignment;
	DROP TABLE #ResponseList;
	EXEC sp_xml_removedocument @hDoc;

	-- latency logging
	SET @EndDate = GETDATE()	
	-- Removed comment on successful entry, makes it easier to correlate to log fails.
	EXEC dbo.sp_WritedbLatency 'dbo.sp_SaveAssignmentAndResponses', @StartDate, @EndDate
	END TRY
	BEGIN CATCH
    	SET @EndDate = GETDATE()	
		ROLLBACK
		SET @Err = ERROR_MESSAGE()
		SET @ErrorFlag = 1
		EXEC dbo.sp_WritedbLatency 'dbo.sp_SaveAssignmentAndResponses', @StartDate, @EndDate, @Err
		RETURN
	END CATCH			
	COMMIT TRANSACTION
	SELECT @ErrorFlag -- 0 indicates success; 1 indicates failure
	
END

