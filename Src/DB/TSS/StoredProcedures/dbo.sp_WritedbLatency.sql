-- =============================================
-- Author:		Sai V.
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[sp_WritedbLatency]
	@ProcName	VARCHAR(200)
  ,	@StartDate	DATETIME
  , @EndDate	DATETIME
  , @Comments	VARCHAR(8000) = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @Duration INT
	SET @Duration = DATEDIFF(ms, @StartDate, @EndDate)

	-- write latency logs only when the time taken to execute the procedure is greater than 500 ms
	-- the if condition is optional; it is done to restrict number of log entries 
	-- IF (@Duration > 500) --ms	
		INSERT INTO dbo._dbLatency (ProcName, StartDate, EndDate, Duration_ms, Comments)
			VALUES (@ProcName, @StartDate, @EndDate, @Duration, @Comments)
		
END
