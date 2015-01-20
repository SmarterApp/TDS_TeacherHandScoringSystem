
/*
	Description: SAVES ASSIGNMENT AND RESPONSE DATA TO THE APPROPRIATE TABLES
	Author: Sai V.
	DATE: 1/5/2015

	NotScored = 0
    TentativeScore = 1
	Scored = 2
  	
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
	DECLARE @Comment	VARCHAR(8000)
	DECLARE @RespFlag	BIT	

	SET @StartDate = GETDATE()
	
	DECLARE @hDoc		INT
	DECLARE @ErrorFlag	BIT
	SET @ErrorFlag = 0

	EXEC sp_xml_preparedocument @hDoc OUTPUT, @xmlInputs

	-- extract data from XML
    SELECT *
    INTO #Assignment
    FROM OPENXML (@hDoc, '/Root/Assignment') 
	WITH ( TestId			VARCHAR(255)
		 , TeacherId		NVARCHAR(250) 
		 , StudentId		BIGINT
		 , SchoolId			VARCHAR(100)
		 , SessionId		NVARCHAR(240)
		 , OpportunityId	BIGINT
		 , OpportunityKey	UNIQUEIDENTIFIER
		 , ClientName		VARCHAR(100)
		 , CallbackUrl		NVARCHAR(MAX)) AS x		--score status should be part of item xml attribute ?? check with Lynn ??


    SELECT *
		 , NEWID() AS ResponseID 
    INTO #ResponseList
    FROM OPENXML (@hDoc, '/Root/ItemList/Item') 
	WITH ( ItemKey		INT				
		 , BankKey		INT
		 , ContentLevel NVARCHAR(MAX)
		 , Format		NVARCHAR(MAX)
		 , SegmentId	NVARCHAR(MAX)	
		 , ScoreStatus	INT
		 , ResponseDate DATETIME
		 , Response		VARCHAR(MAX) 'Response/.') AS x


	/*	SELECT * FROM #Assignment
		SELECT * FROM #ResponseList
	*/

	-- check if the response data is already loaded
	-- set the flag to 0 if rows does not exists, 1 if row exists
	  SET @RespFlag = (CASE WHEN (SELECT COUNT(*) FROM dbo.Assignments a
								 INNER JOIN  #Assignment t
						         ON t.TestID = a.TestID AND t.StudentID = a.StudentID AND t.TeacherID = a.TeacherID 
						         AND t.OpportunityKey = a.OpportunityKey) > 0 THEN 1
						 ELSE 0 END)											   
							
	--SELECT @RespFlag

	-- USE CASE#1: check if data already exists in the database, and if the scored status is set to 'Scored' for all responses in the XML
	-- if conditions satisfy, do nothing
	IF @RespFlag = 1 AND NOT EXISTS (SELECT 1 FROM #ResponseList WHERE ScoreStatus != 2)
	BEGIN
		--PRINT 'USE CASE#1'
		SET @EndDate = GETDATE()	
		EXEC dbo.sp_WritedbLatency 'dbo.sp_SaveAssignmentAndResponses', @StartDate, @EndDate, 'SP exited conditionally. Response data already exists.'
		RETURN		
	END	


	-- USE CASE#2: check if data already exists in the database
	-- @RespFlag = 0 indicates record does not exist. Insert data into the appropriate tables
	IF @RespFlag = 0
	BEGIN
	BEGIN TRANSACTION	
		--PRINT 'USE CASE#2'
		
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

		-- delete from #ResponseList items that are not handscored
		-- we do not need to create assignment rows for such data
		DELETE r
		FROM #ResponseList r
			JOIN dbo.Items i ON i.BankKey = r.BankKey AND i.ItemKey = r.ItemKey
		WHERE i.HandScored = 0

				
		INSERT INTO [dbo].[Assignments]
			   ([SessionId]
			   ,[OpportunityId]
			   ,[OpportunityKey]
			   ,[ScoreStatus]
			   ,[CallbackUrl]
			   ,[ClientName]
			   ,[ResponseID]
			   ,[TeacherID]
			   ,[SchoolID]
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
			 , SchoolID
			 , StudentID
			 , TestID
		FROM #Assignment a, #ResponseList r
			
	COMMIT TRANSACTION
	END								

	
	-- USE CASE#3: check if data already exists in the database and the scoreStatus for each of the responses	
	-- If scoreStatus for a given response is 'Scored' => update response and scoreStatus data
	-- If scoreStatus for a given response is NOT 'Scored' => update only response data
	IF @RespFlag = 1
	BEGIN
		--PRINT 'USE CASE#3'
		
		DECLARE @UpdateToResponseData TABLE (
			AssignmentID		UNIQUEIDENTIFIER
		  , ResponseID			UNIQUEIDENTIFIER
		  , ScoreStatus			INT
		  , New_ScoreStatus		INT
		  , New_Response		VARCHAR(MAX)
		  , New_ResponseDate	DATETIME 
		)
		
		INSERT INTO @UpdateToResponseData		
		SELECT a.AssignmentID
			 , a.ResponseID
			 , a.ScoreStatus
			 , rl.ScoreStatus   AS New_ScoreStatus
			 , rl.Response		AS New_Response 
			 , rl.ResponseDate	AS New_ResponseDate	 
		FROM dbo.Assignments a
			JOIN #Assignment t ON t.TestID = a.TestID AND t.StudentID = a.StudentID AND t.TeacherID = a.TeacherID AND t.OpportunityKey = a.OpportunityKey
			JOIN dbo.Responses r ON r.ResponseID = a.ResponseID
			JOIN #ResponseList rl ON rl.ItemKey = r.ItemKey AND rl.BankKey = r.BankKey AND rl.SegmentID = r.SegmentID
		WHERE rl.ScoreStatus != 2 --Ignore responses that have scoreStatus = 'Scored' on XML		
			 
		-- irrespective of whether the response is scored in THSS or not update the response		
		UPDATE r
		SET Response = u.New_Response
		  , ResponseDate = u.New_ResponseDate
		FROM dbo.Responses r
			JOIN @UpdateToResponseData u ON u.ResponseID = r.ResponseID
			
		-- if response has 'Scored' status in THSS, reset the status
		UPDATE a
		SET ScoreStatus = 0 --NOT SCORED
		  , ScoreData   = NULL 
		FROM dbo.Assignments a
			JOIN @UpdateToResponseData u ON u.AssignmentID = a.AssignmentID
		WHERE a.ScoreStatus = 2 --SCORED
				
	END

	
	IF @@TRANCOUNT > 0
	BEGIN
		ROLLBACK TRANSACTION
		SET @ErrorFlag = 1
	END	

	-- clean-up
	DROP TABLE #Assignment;
	DROP TABLE #ResponseList;
	EXEC sp_xml_removedocument @hDoc;


	SELECT @ErrorFlag -- 0 indicates success; 1 indicates failure

	-- latency logging
	SET @EndDate = GETDATE()	
	SET @Comment = '@ErrorFlag:' + (CASE @ErrorFlag WHEN 0 THEN 'Success' ELSE 'Failure' END)
	EXEC dbo.sp_WritedbLatency 'dbo.sp_SaveAssignmentAndResponses', @StartDate, @EndDate, @Comment
		           
END
