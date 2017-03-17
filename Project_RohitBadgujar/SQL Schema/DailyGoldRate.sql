
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='[DailyGoldRate]' AND xtype='U')
    CREATE TABLE [DailyGoldRate](
	Price DECIMAL(15, 5) NOT NULL,
	LastUpdatedDate Date NOT NULL,
)
GO


