USE [AtmProject]
GO
/****** Object:  StoredProcedure [dbo].[DepositFunds]    Script Date: 8/29/2023 3:34:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[DepositFunds] (
    @user_id INT,
    @accountId INT,
    @amount FLOAT
) AS
BEGIN
    -- Check if the amount is positive
    IF @amount <= 0
    BEGIN
        RAISERROR('Invalid amount. Amount must be positive.', 16, 1);
        RETURN;
    END;

    -- Update the account balance
    UPDATE tbl_Accounts
    SET account_balance = account_balance + @amount
    WHERE account_id = @accountId;

    -- Insert a transaction record
    INSERT INTO tbl_Transactions (User_id, accountTo, Transaction_type, Amount, Date)
    VALUES (@user_id, @accountId, 'Deposit', @amount, GETDATE());

    -- Return success status
    SELECT 'Deposit successful' AS Result;
END;
