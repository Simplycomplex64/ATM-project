USE AtmProject
GO

Alter PROCEDURE WithdrawMoney
    @account_id INT,
    @amount DECIMAL(10, 2)
AS
BEGIN
    DECLARE @current_balance DECIMAL(10, 2);
    DECLARE @overdraft_enabled BIT;
    DECLARE @overdraft DECIMAL(10, 2);

    -- Get current balance and overdraft_enabled status for the account
    SELECT @current_balance = account_balance, @overdraft_enabled = overdraft_enabled, @overdraft = overdraft
    FROM [dbo].[tbl_Accounts]
    WHERE account_id = @account_id;

    -- Check if withdrawal amount is greater than the account balance
    IF @amount > @current_balance
    BEGIN
        IF @overdraft_enabled = 0
        BEGIN
            RAISERROR('Insufficient funds.', 16, 1);
            RETURN;
        END
        ELSE IF @overdraft_enabled = 1
        BEGIN
            -- Calculate the total withdrawal with overdraft
            DECLARE @total_withdrawal DECIMAL(10, 2);
            SET @total_withdrawal = @amount - @current_balance;

			 -- Update the overdraft column
            UPDATE [dbo].[tbl_Accounts]
            SET overdraft += account_balance - @amount
            WHERE account_id = @account_id;
            
            -- Add the overdraft amount to the account balance
            UPDATE [dbo].[tbl_Accounts]
            SET account_balance = 0
            WHERE account_id = @account_id;

            -- Insert the transaction record into tbl_Transactions
            INSERT INTO [dbo].[tbl_Transactions] ([User_id], [accountFrom], [Transaction_type], [Amount], [Date])
            VALUES (@account_id, @account_id, 'Overdraft', @overdraft, GETDATE());
        END
    END
    ELSE
    BEGIN
        -- Start a transaction for the withdrawal
        BEGIN TRANSACTION;

        -- Update the account balance after withdrawal
        UPDATE [dbo].[tbl_Accounts]
        SET account_balance = account_balance - @amount
        WHERE account_id = @account_id;

        -- Insert the transaction record into tbl_Transactions
        INSERT INTO [dbo].[tbl_Transactions] ([User_id], [accountFrom], [Transaction_type], [Amount], [Date])
        VALUES (@account_id, @account_id, 'Withdrawal', @amount, GETDATE());

        -- Commit the transaction
        COMMIT TRANSACTION;

        PRINT 'Withdrawal successful.';
    END;
END;
