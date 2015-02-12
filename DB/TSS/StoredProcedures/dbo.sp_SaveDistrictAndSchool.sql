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

CREATE PROCEDURE [dbo].[sp_SaveDistrictAndSchool] 
    @xmlInputs XML	  
AS
BEGIN 

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @StartDate  DATETIME
	DECLARE @EndDate	DATETIME
	DECLARE @Comment	VARCHAR(8000)	

	SET @StartDate = GETDATE()
	
	DECLARE @hDoc		INT
	DECLARE @ErrorFlag	BIT
	SET @ErrorFlag = 0

	EXEC sp_xml_preparedocument @hDoc OUTPUT, @xmlInputs

	-- extract data from XML
    SELECT *
    INTO #School
    FROM OPENXML (@hDoc, '/Root/School') 
	WITH ( SchoolId		VARCHAR(100)
		 , SchoolName	NVARCHAR(500) 
		 , StateName	NVARCHAR(500)) AS x

    SELECT *
    INTO #District
    FROM OPENXML (@hDoc, '/Root/District') 
	WITH ( DistrictId	VARCHAR(100)
		 , DistrictName	VARCHAR(500)) AS x


	BEGIN TRANSACTION	
		-- check if the district data already exists, if not write to the tables
		INSERT INTO [dbo].[Districts]([DistrictID], [DistrictName])
		SELECT DistrictID
			 , DistrictName
		FROM #District temp
		WHERE NOT EXISTS (SELECT 1
						  FROM [dbo].[Districts] d
						  WHERE d.DistrictID = temp.DistrictID)		 			   

		-- check if the school data already exists, if not write to the tables
		INSERT INTO [dbo].[Schools]
			   ( [SchoolID]
			   , [SchoolName]
			   , [StateName]
			   , [DistrictID])
		SELECT SchoolID
			 , SchoolName
			 , StateName
			 , (SELECT DistrictID FROM #District)
		FROM #School temp		
		WHERE NOT EXISTS (SELECT 1
						  FROM [dbo].[Schools] s
						  WHERE s.SchoolID = temp.SchoolID)			
	COMMIT TRANSACTION
	
	IF @@TRANCOUNT > 0
	BEGIN
		ROLLBACK TRANSACTION
		SET @ErrorFlag = 1
	END	

	-- clean-up
	DROP TABLE #School;
	DROP TABLE #District;
	EXEC sp_xml_removedocument @hDoc;


	SELECT @ErrorFlag -- 0 indicates success; 1 indicates failure

	-- latency logging
	SET @EndDate = GETDATE()	
	SET @Comment = '@ErrorFlag:' + (CASE @ErrorFlag WHEN 0 THEN 'Success' ELSE 'Failure' END)
	EXEC dbo.sp_WritedbLatency 'dbo.sp_SaveDistrictAndSchool', @StartDate, @EndDate, @Comment
		           
END
