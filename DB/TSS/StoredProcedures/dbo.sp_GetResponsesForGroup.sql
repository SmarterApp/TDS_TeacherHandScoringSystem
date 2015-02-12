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

/*
	Description: Save exported item data to the item table
	Author: Aaron
	DATE: 1/8/2015
  	
*/
            
CREATE PROCEDURE [dbo].[sp_GetResponsesForGroup] 
    @Assignment uniqueidentifier,
    @Passage int
AS
BEGIN 

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @StartDate  DATETIME
	DECLARE @EndDate	DATETIME	

	SET @StartDate = GETDATE()
	
-- get the test segment (opportunity) id, so we can tell this response from 
	-- other responses for the same item.
	declare @opportunity uniqueidentifier
	select top 1 @opportunity=assign.OpportunityKey from dbo.Assignments assign
	where assign.AssignmentID=@Assignment
	
	-- Get all the responses for these items on this test.
    declare @responseInfo table (row bigint,_bkey int,_ikey int,response varchar(max),responseDate  DATETIME)

	insert into @responseInfo 
	select 
	ROW_NUMBER() over (order by 
	Items.ItemKey,responses.responseDate) as rownum, 
	items.BankKey,Items.ItemKey,
	Responses.Response, responses.responseDate
	from dbo.Assignments assign 
	inner join dbo.Responses responses on assign.ResponseID=responses.ResponseID
	inner join dbo.Items items on items.Passage=@Passage and items.ItemKey=responses.ItemKey and 
	    items.BankKey=responses.BankKey
	where assign.OpportunityKey=@opportunity
	order by Items.ItemKey,responses.responseDate desc

    -- we only want the most recent response for each item
    -- sicne we sorted on date this is the lowest row
	declare @recentRows table (row bigint,_bkey int,_ikey int)
	insert into @recentRows 
	select MIN(row),_bkey,_ikey from @responseInfo group by _bkey,_Ikey

	select info._bkey as BankKey,info._ikey as ItemKey,response as Response from @responseInfo info
	join @recentRows recent on recent.row=info.row
	
	 	-- latency logging
	SET @EndDate = GETDATE()		  	
	EXEC dbo.sp_WritedbLatency 'dbo.sp_GetResponsesForGroup', @StartDate, @EndDate
	
END								



