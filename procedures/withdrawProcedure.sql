USE [AtmProject]
GO
/****** Object:  StoredProcedure [dbo].[WithdrawProcedure]    Script Date: 8/29/2023 3:37:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[WithdrawProcedure]
    @account_id INT,
    @amount DECIMAL(10, 2),
	@user_id bigint
AS
BEGIN
    DECLARE @current_balance DECIMAL(10, 2);
    DECLARE @overdraft_enabled BIT;
    DECLARE @overdraft DECIMAL(10, 2);
    DECLARE @paper_funds DECIMAL(10, 2);

    -- Get information from the account
    SELECT @current_balance = account_balance, @overdraft_enabled = overdraft_enabled, @overdraft = overdraft
    FROM [dbo].[tbl_Accounts]
    WHERE account_id = @account_id;

    -- Get paper funds
    SELECT @paper_funds = paperFunds
    FROM [dbo].[tbl_paperFunds]

    -- Check if withdrawal amount is higher than paper funds
    IF @amount > @paper_funds
    BEGIN
        RAISERROR('Insufficient funds in the ATM.', 16, 1);
        RETURN;
    END
    ELSE
    BEGIN
        -- Check if withdrawal amount is higher than the account balance
        IF @amount > @current_balance
        BEGIN
            IF @overdraft_enabled = 1
            BEGIN
                DECLARE @required_overdraft DECIMAL(10, 2);
                SET @required_overdraft = @amount - @current_balance;

                -- Start a transaction for overdraft transfer
                BEGIN TRANSACTION;

                -- Update overdraft amount
                UPDATE [dbo].[tbl_Accounts]
                SET overdraft -= @required_overdraft
                WHERE account_id = @account_id;

				 -- Update account balance after withdrawal
            UPDATE [dbo].[tbl_Accounts]
            SET account_balance = 0
            WHERE account_id = @account_id;

                -- Update paper funds
                UPDATE [dbo].[tbl_paperFunds]
                SET paperFunds -= @amount

                -- Insert transaction record for overdraft transfer
                INSERT INTO [dbo].[tbl_Transactions] ([User_id], [accountFrom], [Transaction_type], [Amount], [Date])
                VALUES (@user_id, @account_id, 'Overdraft Withdrawal', @required_overdraft, GETDATE());

                -- Commit the overdraft transaction
                COMMIT TRANSACTION;

                PRINT 'Withdrawal successful using overdraft.';
            END
            ELSE
            BEGIN
                RAISERROR('Insufficient funds for withdrawal.', 16, 1);
                RETURN;
            END
        END
        ELSE
        BEGIN
            -- Start a transaction for the withdrawal
            BEGIN TRANSACTION;

            -- Update paper funds
            UPDATE [dbo].[tbl_paperFunds]
            SET paperFunds -= @amount

            -- Update account balance after withdrawal
            UPDATE [dbo].[tbl_Accounts]
            SET account_balance -= @amount
            WHERE account_id = @account_id;

            -- Insert transaction record into tbl_Transactions
            INSERT INTO [dbo].[tbl_Transactions] ([User_id], [accountFrom], [Transaction_type], [Amount], [Date])
            VALUES (@user_id, @account_id, 'Withdrawal', @amount, GETDATE());

            -- Commit the transaction
            COMMIT TRANSACTION;

            PRINT 'Withdrawal successful.';
        END;
    END;
END;