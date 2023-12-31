USE [AtmProject]
GO
/****** Object:  StoredProcedure [dbo].[AccountOverview]    Script Date: 8/29/2023 3:31:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[AccountOverview]
    @userId INT
AS
BEGIN
    SELECT account_id, account_type, ROUND(account_balance, 2) AS account_balance
    FROM tbl_Accounts
    WHERE User_id = @userId;
END;
