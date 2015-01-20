
CREATE PROCEDURE [dbo].[sp_GetItemConfigurations] 
AS
BEGIN
 SELECT * FROM dbo.ConditionCodes;
 
 SELECT * FROM dbo.Dimensions;
 
 SELECT * FROM dbo.Items;
 
END
