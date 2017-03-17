IF  EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_NAME = '[UpdateDailyGoldPrice]')
BEGIN
 DROP PROCEDURE UpdateDailyGoldPrice
END 
GO
CREATE PROC UpdateDailyGoldPrice(
@Price Decimal(15,5), -- INPUT Parameter Gold Rate 
@Day DateTime,		  -- INPUT Parameter Day 
@Flag INT  = NULL OUTPUT  -- Output Parameter Flag  
)

AS

BEGIN
SET @Flag  = (SELECT Price FROM DailyGoldRate WHERE LastUpdatedDate = CAST(@Day as Date))
IF @Flag IS NOT NULL
	BEGIN
		UPDATE DailyGoldRate SET Price = cast(@Price  AS FLOAT) WHERE LastUpdatedDate = CAST(@Day as Date) -- Update IF Date EXIST
		SET @Flag = 1
	END
ELSE
	BEGIN
		INSERT INTO DailyGoldRate VALUES (cast(@Price  AS FLOAT),@day) -- -- INSERT IF NEW RECORD
		SET @Flag = 0
	END
	RETURN @Flag        -- Update/Insert Flag.
END
