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
            
CREATE PROCEDURE [dbo].[sp_UpdateCodes] 
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
	declare @Updates TABLE (
	DimensionId int,
        FullName varchar(max),
        ShortName varchar(max),
	    DimensionName varchar(max),
	    ConditionCodeId int,
	    ItemKey int,
	    BankKey int	    
	)

    	
	-- extract data from XML
	insert into @Updates
    SELECT 
	      0,  M.Codes.query('./FullName').value('.','nvarchar(max)') FullName
	     ,   M.Codes.query('./ShortName').value('.','nvarchar(max)') ShortName
	     ,   M.Codes.query('./DimensionName').value('.','nvarchar(max)') DimensionName
	     ,   M.Codes.query('./ConditionCodeId').value('.','INT') ConditionCodeId
	     ,   M.Codes.query('./ItemKey').value('.','INT') ItemKey
	     ,   M.Codes.query('./BankKey').value('.','INT') BankKey
    FROM
      @xmlUpdates.nodes('/ArrayOfConditionCodeSql/ConditionCodeSql') AS M(Codes)
                  
       
    -- Update those records   
	UPDATE codes set codes.FullName=updates.FullName,
	codes.ShortName=updates.ShortName
	
	from dbo.ConditionCodes codes 
	join @Updates updates on updates.ConditionCodeId = codes.ConditionCodeId
	where updates.ConditionCodeId > 0
	
	-- The ones we have just updated have index > 0.  Remove them
	-- since we just updated those
	delete from @Updates where ConditionCodeId > 0
	
	-- The dimension codes may not be correct if this is a new dimension.
	-- so next step is to find the dimensions since we need to have the correct 
	-- index (we add those first).
	UPDATE updates set DimensionId=dims.DimensionId
	from @Updates updates
	join dbo.Dimensions dims on updates.DimensionName = dims.Name
	  and dims.ItemKey = updates.ItemKey and dims.BankKey = updates.BankKey
	  
	INSERT INTO dbo.ConditionCodes (FullName,ShortName,DimensionId)
	   select updates.FullName,updates.ShortName,updates.DimensionId	
	   from @Updates updates
	   where updates.DimensionId > 0
	   
	 	-- latency logging
	SET @EndDate = GETDATE()		  	
	EXEC dbo.sp_WritedbLatency 'dbo.sp_UpdateDimensions', @StartDate, @EndDate
	
END								


