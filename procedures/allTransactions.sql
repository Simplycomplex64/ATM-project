USE [AtmProject]
GO
/****** Object:  StoredProcedure [dbo].[allTransactions]    Script Date: 8/29/2023 3:32:14 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[allTransactions]
    @user_id bigint
AS
BEGIN
    SELECT * FROM tbl_Transactions WHERE User_id = @user_id;
END
