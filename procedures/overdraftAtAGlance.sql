USE [AtmProject]
GO
/****** Object:  StoredProcedure [dbo].[overdraftAtAGlance]    Script Date: 8/29/2023 3:36:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[overdraftAtAGlance]
    @User_id INT,
    @account_type NVARCHAR(50)
AS
BEGIN
    SELECT overdraft
    FROM tbl_Accounts
    WHERE User_id = @User_id AND account_type = @account_type;
END;
