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
	Description: Produce a report that indicates load
	Author: Aaron
	DATE:2/19/2015

    Input: @Day1 - the beginning of the work week, e.g. 2015-02-09 15:00:00.000
      is around 6am PST
*/

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
	Description: Select some sprocs and calculate occurence and 
	latency for load evaluation.
	Author: Aaron
	DATE:2/3/2015


*/
CREATE PROCEDURE [dbo].[sp_GetActivityReport] 
	@Day1		DATETIME
AS BEGIN

-- Create a static table of the hours in work day (14, AIR SWE hours...)
DECLARE @hoursTable TABLE (htime bigint)
INSERT INTO @hoursTable SELECT * FROM (VALUES 
(0),
(1), 
(2), 
(3), 
(4), 
(5), 
(6), 
(7), 
(8), 
(9), 
(10), 
(11), 
(12), 
(13), 
(14), 
(15), 
(16) 
) AS dayHours (a)


DECLARE @intervals TABLE (startTime DateTime,stopTime DateTime)

-- Get 1-hour workday intervals for the whole week.
insert into @intervals select DateAdd(hour,htime,@day1),
       DateAdd(hour,htime+1,@day1) from @hoursTable hhh
insert into @intervals select DateAdd(hour,htime+24,@day1),
       DateAdd(hour,htime+25,@day1) from @hoursTable hhh
insert into @intervals select DateAdd(hour,htime+48,@day1),
       DateAdd(hour,htime+49,@day1) from @hoursTable hhh
insert into @intervals select DateAdd(hour,htime+72,@day1),
       DateAdd(hour,htime+73,@day1) from @hoursTable hhh
insert into @intervals select DateAdd(hour,htime+96,@day1),
       DateAdd(hour,htime+97,@day1) from @hoursTable hhh

-- We query 2 sprocs, one that lists the items and one that 
-- scores a single assignment, to get an idea.
select [DayNum],[Day],[Hour],Count(*) as [Views] ,AVG([Duration]) as [AvgDuration] from (
select [DayNum]=DATEPART(day,latency.StartDate),
       [Day]=datename(dw,latency.startdate),
       [Hour]=DATEPART(hour,latency.StartDate),
       [Duration]=latency.Duration_ms
from dbo._dbLatency latency 
join @intervals intervals on intervals.startTime<latency.StartDate
and intervals.stopTime > latency.EndDate
where latency.ProcName like 'dbo.sp_GetItemList'
) as Q
group by [DayNum],[Day],[Hour] order by DayNum,Hour asc

select [DayNum],[Day],[Hour],Count(*) as [Scores],AVG([Duration]) as [AvgDuration]  from (
select [DayNum]=DATEPART(day,latency.StartDate),
       [Day]=datename(dw,latency.startdate),
       [Hour]=DATEPART(hour,latency.StartDate),
       [Duration]=latency.Duration_ms
from dbo._dbLatency latency 
join @intervals intervals on intervals.startTime<latency.StartDate
and intervals.stopTime > latency.EndDate
where latency.ProcName like 'dbo.sp_UpdateAssignmentScore'
) as Q
group by [DayNum],[Day],[Hour] order by DayNum,Hour asc

END
