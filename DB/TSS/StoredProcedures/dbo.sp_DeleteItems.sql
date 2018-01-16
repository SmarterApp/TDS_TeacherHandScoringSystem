USE [TSS]
GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteItems]    Script Date: 1/15/2018 3:44:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
    Description: Delete item config
    Deletes a list of comma delimited item keys using the specified bank key
*/
ALTER PROCEDURE [dbo].[sp_DeleteItems]
    @BankKey        INT
   ,@ItemKeyList    VARCHAR(MAX)

AS
BEGIN

    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;
        DECLARE @StartDate  DATETIME
    DECLARE @EndDate	DATETIME

    SET @StartDate = GETDATE()

    CREATE TABLE #ItemKeyList(
        ItemKey int
    )

    INSERT INTO #ItemKeyList
    SELECT CAST(items as int)
    FROM dbo.fn_SplitDelimitedString(@ItemKeyList, ',')

    DELETE conditions FROM dbo.ConditionCodes conditions
        INNER JOIN dbo.Dimensions dimensions ON
            dimensions.BankKey = @BankKey and
            conditions.DimensionId = dimensions.DimensionId
        INNER JOIN  #ItemKeyList itemKeyList ON
            dimensions.ItemKey = itemKeyList.ItemKey

    DELETE dimensions FROM dbo.Dimensions dimensions
        INNER JOIN  #ItemKeyList itemKeyList ON
            dimensions.BankKey = @BankKey and
            dimensions.ItemKey = itemKeyList.ItemKey

    DELETE items FROM dbo.Items items
        INNER JOIN  #ItemKeyList itemKeyList ON
            items.BankKey = @BankKey and
            items.ItemKey = itemKeyList.ItemKey

    -- latency logging
    SET @EndDate = GETDATE()
    EXEC dbo.sp_WritedbLatency 'dbo.sp_DeleteItems', @StartDate, @EndDate

END

