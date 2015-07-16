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
/****** Object:  StoredProcedure [dbo].[sp_UpdateDistrictTeacherRelationship]    Script Date: 02/02/2015 19:19:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [dbo].[sp_UpdateDistrictTeacherRelationship] 
    @TeacherId		VARCHAR(100)
  , @DistrictId     VARCHAR(250)
AS
BEGIN 

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @StartDate  DATETIME
	DECLARE @EndDate	DATETIME	

	SET @StartDate = GETDATE()
	
	DECLARE @ErrorFlag	BIT
	DECLARE @Err		VARCHAR(8000)
	SET @ErrorFlag = 0
	
	IF NOT EXISTS (SELECT 1 FROM dbo.TeacherDistrictMap WHERE  TeacherID = @TeacherId
	   and DistrictID = @DistrictId)
	BEGIN	  
    	 Insert into dbo.TeacherDistrictMap values (@DistrictId,@TeacherId)    	 
  	END
	
	SELECT @ErrorFlag -- 0 indicates success; 1 indicates failure

	-- latency logging
	SET @EndDate = GETDATE()
	EXEC dbo.sp_WritedbLatency 'dbo.sp_UpdateDistrictTeacherRelationship', @StartDate, @EndDate
	           
END


