USE [AtmProject]
GO
/****** Object:  StoredProcedure [dbo].[ClearOverdraftFromBalance]    Script Date: 8/23/2023 5:50:13 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[ClearOverdraftFromBalance]
    @userId INT
AS
BEGIN
    DECLARE @accountBalance DECIMAL(18, 2);
    DECLARE @overdraft DECIMAL(18, 2);
	DECLARE @account_id bigint;

    -- Get the account balance and overdraft amount for the user's checking account
    SELECT @accountBalance = account_balance, @overdraft = overdraft, @account_id = account_id
    FROM tbl_Accounts
    WHERE User_id = @userId AND account_type = 'Checking';
    BEGIN
        BEGIN TRANSACTION;

        -- Deduct the overdraft from the account balance
        UPDATE tbl_Accounts
        SET account_balance = account_balance + (overdraft)
        WHERE User_id = @userId AND account_type = 'Checking';

		UPDATE tbl_Accounts
        SET overdraft = 0
        WHERE User_id = @userId AND account_type = 'Checking';

        -- Insert a record in the transaction table
        INSERT INTO tbl_Transactions (User_id, accountFrom, transaction_type, Amount, Date)
        VALUES (@userId, @account_id, 'Clear Overdraft', @overdraft, GETDATE());

        COMMIT;

        PRINT 'Overdraft has been paid successfully.';
    END
END;
