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
-- USE [TSS_Debug_1]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
	Description: Update item config from xml
	Author: aaron
	DATE: 1/18/2015
  	
*/
            
CREATE PROCEDURE [dbo].[sp_UpdateDimensions] 
    @xmlUpdates	 XML   	  
AS
BEGIN 

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
		DECLARE @StartDate  DATETIME
	DECLARE @EndDate	DATETIME	

	SET @StartDate = GETDATE()
	
	-- Create a temp table
	declare @Dimensions TABLE (
        DimensionId int,
        Name varchar(max),
        Min int,
        Max int,
        ItemKey int,
        BankId int
	)
    insert into [dbo].[DoNotCopyAaronDbg] select @xmlUpdates
    
	-- extract data from XML
	insert into @Dimensions
    SELECT 
	     M.Dimension.query('./DimensionId').value('.','INT') DimensionId
	     ,   M.Dimension.query('./Name').value('.','varchar(max)') Name
	     ,   M.Dimension.query('./Min').value('.','INT') Min
	     ,   M.Dimension.query('./Max').value('.','INT') Max
	     ,   M.Dimension.query('./ItemKey').value('.','INT') ItemKey
	     ,   M.Dimension.query('./BankId').value('.','INT') BankId
    FROM
      @xmlUpdates.nodes('/ArrayOfDimension/Dimension') AS M(Dimension)
    
    -- for data that is updating current rows, update the temp table with
    -- those row keys
    UPDATE updates set updates.DimensionId = dims.DimensionId
    from @Dimensions updates 
    join dbo.Dimensions dims on dims.BankKey=updates.BankId and
       dims.ItemKey=updates.ItemKey and 
       dims.Name = updates.Name
              
    -- UPdate those records   
	UPDATE dims set dims.BankKey=updates.BankId,
	dims.ItemKey=updates.ItemKey,
	dims.Name=updates.Name,
	dims.Min=updates.Min,
	dims.Max=updates.Max
	
	from dbo.Dimensions dims 
	join @Dimensions updates on updates.DimensionID = dims.DimensionId
	where updates.DimensionID > 0
	
	-- The ones we have just updated have index > 0.  Remove them
	-- since we just updated those
	delete from @dimensions where DimensionId > 0
		
    -- what is left over are adds.  Add them.
	INSERT INTO dbo.Dimensions  (Name,BankKey,ItemKey,Min,Max)
	   select Name,BankId,ItemKey,Min,Max
	   from @dimensions
	
	 	-- latency logging
	SET @EndDate = GETDATE()		  	
	EXEC dbo.sp_WritedbLatency 'dbo.sp_UpdateDimensions', @StartDate, @EndDate
	
END								

