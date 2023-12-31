USE [AtmProject]
GO
/****** Object:  StoredProcedure [dbo].[CreateAccountForUser]    Script Date: 8/29/2023 3:33:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[CreateAccountForUser]
(
    @AccountType VARCHAR(50),
    @UserID INT
)
AS
BEGIN
    DECLARE @AccountID INT
    DECLARE @OverdraftEnabled BIT

    -- Set overdraft_enabled based on account type
    IF @AccountType = 'Checking'
        SET @OverdraftEnabled = 1;  -- Overdraft enabled for Checking
    ELSE IF @AccountType = 'Saving'
        SET @OverdraftEnabled = 0;  -- Overdraft not enabled for Saving

    -- Insert into Accounts table
    INSERT INTO tbl_Accounts (account_type, account_balance, status, User_id, overdraft_enabled, overdraft)
    VALUES (@AccountType, 0, 1, @UserID, @OverdraftEnabled, 0)

    SET @AccountID = SCOPE_IDENTITY()  -- Get the auto-generated Account ID

    SELECT @AccountID AS AccountID  -- Return the newly created Account ID
END
