USE [AtmProject]
GO
/****** Object:  StoredProcedure [dbo].[SearchTransactionsByUser]    Script Date: 8/29/2023 3:36:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[SearchTransactionsByUser]
    @User_id INT
AS
BEGIN
    SELECT
        [Transaction_id],
        [User_id],
        [accountFrom],
        [accountTo],
        [Transaction_type],
        [Amount],
        [Date]
    FROM
        tbl_Transactions
    WHERE
        [User_id] = @User_id
    ORDER BY
        [Date] DESC; -- Order by date in descending order to show recent transactions first
END;
