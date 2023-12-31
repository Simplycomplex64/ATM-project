USE [AtmProject]
GO
/****** Object:  StoredProcedure [dbo].[TransferProcedure]    Script Date: 8/29/2023 3:36:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[TransferProcedure]
    @from_account_id INT,
    @to_account_id INT,
    @amount DECIMAL(10, 2),
	@user_id bigint
AS
BEGIN
    DECLARE @from_balance DECIMAL(10, 2);
    DECLARE @from_overdraft_enabled BIT;
    DECLARE @from_overdraft DECIMAL(10, 2);

    -- Get information from the source account
    SELECT @from_balance = account_balance, @from_overdraft_enabled = overdraft_enabled, @from_overdraft = overdraft, @user_id = User_id
    FROM [dbo].[tbl_Accounts]
    WHERE account_id = @from_account_id;

    -- Check if the source account has enough balance
    IF @amount > @from_balance
    BEGIN
        IF @from_overdraft_enabled = 1
        BEGIN
            DECLARE @required_overdraft DECIMAL(10, 2);
            SET @required_overdraft = @amount - @from_balance;

            -- Start a transaction for overdraft transfer
            BEGIN TRANSACTION;

            -- Update overdraft amount
            UPDATE [dbo].[tbl_Accounts]
            SET overdraft -= @required_overdraft
            WHERE account_id = @from_account_id;

            -- Insert transaction record for overdraft transfer
            INSERT INTO [dbo].[tbl_Transactions] ([User_id], [accountFrom], [Transaction_type], [Amount], [Date])
            VALUES (@user_id, @from_account_id, 'Overdraft Transfer', @required_overdraft, GETDATE());

            -- Commit the overdraft transaction
            COMMIT TRANSACTION;
            
            -- Proceed with normal transfer
            BEGIN TRANSACTION;

            -- Update source account balance after transfer
            UPDATE [dbo].[tbl_Accounts]
            SET account_balance =0
            WHERE account_id = @from_account_id;

            -- Update target account balance after transfer
            UPDATE [dbo].[tbl_Accounts]
            SET account_balance += @amount
            WHERE account_id = @to_account_id;

            -- Insert transaction records for both accounts
            INSERT INTO [dbo].[tbl_Transactions] ([User_id], [accountFrom], [accountTo], [Transaction_type], [Amount], [Date])
            VALUES (@user_id, @from_account_id, @to_account_id, 'Transfer', @amount, GETDATE());

            INSERT INTO [dbo].[tbl_Transactions] ([User_id], [accountFrom], [accountTo], [Transaction_type], [Amount], [Date])
            VALUES (@user_id, @from_account_id, @to_account_id, 'Deposit', @amount, GETDATE());

            -- Commit the transaction
            COMMIT TRANSACTION;

            PRINT 'Transfer successful using overdraft.';
        END
        ELSE
        BEGIN
            RAISERROR('Insufficient funds for transfer.', 16, 1);
            RETURN;
        END
    END
    ELSE
    BEGIN
        -- Start a transaction for the transfer
        BEGIN TRANSACTION;

        -- Update source account balance after transfer
        UPDATE [dbo].[tbl_Accounts]
        SET account_balance -= @amount
        WHERE account_id = @from_account_id;

        -- Update target account balance after transfer
        UPDATE [dbo].[tbl_Accounts]
        SET account_balance += @amount
        WHERE account_id = @to_account_id;

        -- Insert transaction records for both accounts
        INSERT INTO [dbo].[tbl_Transactions] ([User_id], [accountFrom], [accountTo], [Transaction_type], [Amount], [Date])
        VALUES (@user_id, @from_account_id, @to_account_id, 'Transfer', @amount, GETDATE());

        INSERT INTO [dbo].[tbl_Transactions] ([User_id], [accountFrom], [accountTo], [Transaction_type], [Amount], [Date])
        VALUES (@user_id, @from_account_id, @to_account_id, 'Deposit', @amount, GETDATE());

        -- Commit the transaction
        COMMIT TRANSACTION;

        PRINT 'Transfer successful.';
    END;
END;
