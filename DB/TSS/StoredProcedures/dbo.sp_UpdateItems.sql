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
	Description: Update item config from xml
	Author: aaron
	DATE: 1/8/2015
  	
*/
            
CREATE PROCEDURE [dbo].[sp_UpdateItems] 
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
	declare @Items TABLE (
	[BankKey] [int] NOT NULL,
	[ExemplarURL] [nvarchar](max) NULL,
	[ItemKey] [int] NOT NULL,
	[Subject] [nvarchar](max) NULL,
	[Grade] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[RubricListXML] [nvarchar](max) NULL,
	[TrainingGuideURL] [nvarchar](max) NULL,
	[Layout] [nvarchar](max) NULL,
	[Passage] [int] NULL,
    [HandScored] [bit] NULL	
	)

    -- Create a table for updates vs. adds	
	declare @ItemsUpdate TABLE (
     [BankKey] [int] NOT NULL,
     [ItemKey] [int] NOT NULL
    )
	
	-- extract data from XML
	insert into @Items
    SELECT 
	     M.Item.query('./BankKey').value('.','INT') BankKey
	     ,   M.Item.query('./ExemplarURL').value('.','nvarchar(max)') ExemplarURL
	     ,   M.Item.query('./ItemKey').value('.','INT') ItemKey
		 , 	 M.Item.query('./Subject').value('.','nvarchar(max)') Subject
		 , 	 M.Item.query('./Grade').value('.','nvarchar(max)') Grade
		 , 	 M.Item.query('./Description').value('.','nvarchar(max)') Description
		 , 	 M.Item.query('./RubricListXML').value('.','nvarchar(max)') RubricListXML
		 , 	 M.Item.query('./TrainingGuideURL').value('.','nvarchar(max)') TrainingGuideURL
		 , 	 M.Item.query('./Layout').value('.','nvarchar(max)') Layout
    	 ,   M.Item.query('./Passage').value('.','INT') Passage
	     ,   M.Item.query('./HandScored').value('.','BIT') HandScored
    FROM
      @xmlUpdates.nodes('/ArrayOfItemType/ItemType')AS M(Item)
   
   -- Find the items that are already in the table
   Insert into @ItemsUpdate select items.BankKey,items.ItemKey from dbo.Items items
   join @Items updates on updates.BankKey=items.BankKey and items.ItemKey=updates.ItemKey 

    -- UPdate those records   
	UPDATE items set items.BankKey=updates.bankKey,
	items.ItemKey=updates.ItemKey,
	items.ExemplarURL=updates.ExemplarURL,
	items.Subject=updates.Subject,
	items.Grade=updates.Grade,
	items.Description=updates.Description,
	items.RubricListXML=updates.RubricListXML,
	items.TrainingGuideURL=updates.TrainingGuideURL,
	items.Layout=updates.Layout,
	items.Passage=updates.Passage,
	items.HandScored=updates.HandScored
	
	from dbo.Items
	join @Items updates on updates.BankKey=items.BankKey and updates.ItemKey=Items.ItemKey
	
	-- Remove all those updates
	delete updates from @items updates 
	inner join dbo.Items items on items.BankKey=updates.BankKey 
	and items.ItemKey=updates.ItemKey
		
    -- what is left over are adds.  Add them.
	INSERT INTO dbo.Items select * from @Items		
	
	 	-- latency logging
	SET @EndDate = GETDATE()		  	
	EXEC dbo.sp_WritedbLatency 'dbo.sp_UpdateItems', @StartDate, @EndDate
	
END								


