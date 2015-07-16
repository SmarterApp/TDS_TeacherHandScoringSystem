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
	Description: RETURNS A  LIST OF ITEMS TO DISPLAYED ON THE ITEM LIST SCREEN
	Author: TGebicke - Summit
	DATE:12/18/2014
	
	Updated by: Sai - 1/5/2015 - Re-wrote code to optimize performance

*/

CREATE PROCEDURE [dbo].[sp_GetItemList]
	@StartRow		INT
  ,	@EndRow			INT
  ,	@SortColumn		VARCHAR(50) = NULL
  ,	@SortDirection	VARCHAR(50) = NULL
  ,	@EmailList		NVARCHAR(MAX)
  ,	@TestFilter		VARCHAR(255)
  ,	@SessionFilter	NVARCHAR(500)
  ,	@GradeFilter	VARCHAR(50)
  ,	@SubjectFilter	VARCHAR(100)
  ,	@ScorerFilter	NVARCHAR(500)
  , @PassPhrase		VARCHAR(100) = NULL
AS
BEGIN

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @StartDate  DATETIME
	DECLARE @EndDate	DATETIME	

	SET @StartDate = GETDATE()
		
	--CREATE TEMP Table
	CREATE TABLE #IdTable (EmailID VARCHAR(250))

	-- split the e-mail list string
	INSERT INTO #IdTable
	SELECT items
	FROM dbo.fn_SplitDelimitedString(@EmailList, '|')

	-- filter condition logic
	IF(ISNULL(@SortDirection, '') = '')
	  SET @SortDirection = 'ASC'

    DECLARE @RowNumSortColumn VARCHAR(100)
	SET @RowNumSortColumn = @SortColumn
	
	IF(ISNULL(@SortColumn, '') = '')
	BEGIN
	  SET @SortColumn = 'AssignmentId'+ ' ' + @SortDirection
	  SET @RowNumSortColumn = 'a.AssignmentId'+ ' ' + @SortDirection
    END 
    
    IF(ISNULL(@SortColumn, '') = 'ItemKey')
    BEGIN
      SET @SortColumn = 'ItemKey' + ' ' + @SortDirection
	  SET @RowNumSortColumn = 'i.ItemKey'+ ' ' + @SortDirection
	END
	
	 IF(ISNULL(@SortColumn, '') = 'SessionId')
    BEGIN
      SET @SortColumn = 'SessionId' + ' ' + @SortDirection+ ', ' + 'AssignmentId ASC'
	  SET @RowNumSortColumn = 'a.SessionId'+ ' ' + @SortDirection+ ', ' + 'AssignmentId ASC'
	END

    IF(ISNULL(@SortColumn, '') = 'AssignedTo')
    BEGIN
      SET @SortColumn = 'AssignedTo' + ' ' + @SortDirection+ ', ' + 'AssignmentId ASC'
	  SET @RowNumSortColumn = 'te.Name'+ ' ' + @SortDirection+ ', ' + 'AssignmentId ASC'
	END

    IF(ISNULL(@SortColumn, '') = 'ScoreStatus')
    BEGIN
	  SET @SortColumn = 'ScoreStatus' + ' ' + @SortDirection + ', ' + 'AssignmentId ASC'
	  SET     @RowNumSortColumn = 'ScoreStatus' + ' ' + @SortDirection + ', ' + 'AssignmentId ASC'
    END

    IF(ISNULL(@SortColumn, '') = 'StudentName')
    BEGIN 
	  IF (@PassPhrase IS NOT NULL) 
	  BEGIN
	    SET @SortColumn = 'StudentName'+ ' ' + @SortDirection
		SET @RowNumSortColumn = 'dbo.fn_DecryptValue(@PassPhrase, s.Name)'+ ' ' + @SortDirection + ', ' + 'AssignmentId ASC'
		END
	  ELSE 
	  BEGIN
	    SET @SortColumn = 'StudentName' + ' ' + @SortDirection
	    SET @RowNumSortColumn = 's.FirstName + '' '' + s.LastName'+ ' ' + @SortDirection + ', ' + 'AssignmentId ASC'
	    END
	END 
	
	DECLARE @TestFilterCond		VARCHAR(500)
	DECLARE @SessionFilterCond	VARCHAR(500)
	DECLARE @GradeFilterCond	VARCHAR(500)
	DECLARE @SubjectFilterCond	VARCHAR(500)
	DECLARE @ScorerFilterCond	VARCHAR(500)
	
	SET @TestFilterCond		= (CASE WHEN @TestFilter = '' THEN '1 = 1' ELSE 'a.TestID = @TestFilter' END)
	SET @ScorerFilterCond	= (CASE WHEN @ScorerFilter = '' THEN '1 = 1' ELSE 'a.TeacherID = @ScorerFilter' END)
	SET @SessionFilterCond  = (CASE WHEN @SessionFilter = '' THEN '1 = 1' ELSE 'a.SessionId = @SessionFilter' END)
	SET @GradeFilterCond	= (CASE WHEN @GradeFilter = '' THEN '1 = 1' ELSE 't.Grade = @GradeFilter' END)
	SET @SubjectFilterCond  = (CASE WHEN @SubjectFilter = '' THEN '1 = 1' ELSE 't.subject = @SubjectFilter' END)
	

	DECLARE @SQL NVARCHAR(4000)
	SET @SQL = ' SELECT * 
				 FROM ( 
					SELECT ROW_NUMBER() OVER (ORDER BY ' + @RowNumSortColumn +')	AS ROWNUM
						 , te.Name										AS AssignedTo
						 , a.SessionId
						 , a.ScoreStatus
						 , i.ItemKey
						 , i.Description								AS ItemTypeDescription
						 , CASE WHEN @PassPhrase IS NOT NULL THEN dbo.fn_DecryptValue(@PassPhrase, s.Name) ELSE (s.FirstName + '' '' + s.LastName) END AS StudentName
						 , a.AssignmentId
						 , te.TeacherID									AS TeacherUUID 
					FROM dbo.Assignments a (NOLOCK)
						JOIN dbo.Teachers te (NOLOCK) ON te.TeacherID = a.TeacherID
						JOIN dbo.Responses r (NOLOCK) ON r.ResponseID = a.ResponseID
						JOIN dbo.Items i ON i.BankKey = r.BankKey AND i.ItemKey = r.ItemKey						
						JOIN dbo.Students s (NOLOCK) ON s.StudentID = a.StudentID
						JOIN dbo.Tests t (NOLOCK) ON a.TestID = t.TestID
						JOIN #IdTable id ON id.EmailID = a.TeacherID
					WHERE ' + @ScorerFilterCond + ' 
						  AND ' + @TestFilterCond + '
						  AND ' + @SessionFilterCond + '
						  AND ' + @GradeFilterCond + '
						  AND ' + @SubjectFilterCond + '
						  AND a.ScoreStatus < 2	
						  AND (i.HandScored = 1 OR i.HandScored is NULL) 		--should only return hand scorable items
						  --01.13.2015: This filter is no longer needed. Bcoz Assignments are only created for handscored items. 
				  ) AS RESULTSET 
				WHERE ROWNUM > @StartRow 
					AND ROWNUM <= @EndRow 
				ORDER BY ' + @SortColumn ;
				
	--PRINT @SQL
	
	DECLARE @ParamsList NVARCHAR(1000) = N'@StartRow int, @EndRow int, @TestFilter varchar(255), @SessionFilter nvarchar(500)
										 , @GradeFilter varchar(50), @SubjectFilter varchar(100), @ScorerFilter varchar(250), @PassPhrase varchar(100)'
	
	EXECUTE sp_executesql @SQL
	   , @ParamsList 
	   , @StartRow
	   , @EndRow
	   , @TestFilter
	   , @SessionFilter
	   , @GradeFilter
	   , @SubjectFilter
	   , @ScorerFilter
	   , @PassPhrase

	   	   
	-- clean-up
	DROP TABLE #IdTable

	-- latency logging
	SET @EndDate = GETDATE()		  	
	EXEC dbo.sp_WritedbLatency 'dbo.sp_GetItemList', @StartDate, @EndDate
	
END
