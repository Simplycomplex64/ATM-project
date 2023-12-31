USE [AtmProject]
GO
/****** Object:  StoredProcedure [dbo].[MakePayment]    Script Date: 8/29/2023 3:35:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[MakePayment]
    @Bill_id INT,
    @Amount DECIMAL(18, 2),
    @account_id INT,
    @user_id INT,
    @accountFrom INT,
    @accountTo INT,
	@Result INT OUTPUT 
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION; -- Start a transaction
    
    DECLARE @Payment_id INT;

    -- Check if the account balance is sufficient for the payment
    DECLARE @CurrentBalance DECIMAL(18, 2);
    SELECT @CurrentBalance = account_balance FROM tbl_Accounts WHERE account_id = @accountFrom;

    IF @CurrentBalance >= @Amount
    BEGIN
        -- Update the account balance
        UPDATE tbl_Accounts
        SET account_balance = (account_balance - @Amount)-1.25
        WHERE account_id = @accountFrom;

        -- Insert into Payment table
        INSERT INTO tbl_payments (Bill_id, Amount, Date, account_id, user_id)
        VALUES (@Bill_id, @Amount, GETDATE(), @account_id, @user_id);

        SET @Payment_id = SCOPE_IDENTITY(); -- Get the generated Payment_id

        -- Insert into Transaction table (payment as credit to accountFrom and debit to accountTo)
        INSERT INTO tbl_Transactions (User_id, accountFrom, accountTo, Transaction_type, Amount, Date)
        VALUES (@user_id, @accountFrom, @Bill_id, 'Payment', @Amount, GETDATE());
		SET @Result = 0;
        COMMIT; -- Commit the transaction

        RETURN 0; -- Payment successful
    END
    ELSE
    BEGIN
        -- Check if the account allows overdraft
        DECLARE @OverdraftEnabled BIT;
        SELECT @OverdraftEnabled = overdraft_enabled FROM tbl_Accounts WHERE account_id = @accountFrom;

        IF @OverdraftEnabled = 1
        BEGIN
            -- Calculate overdraft amount and update tables
            DECLARE @OverdraftAmount DECIMAL(18, 2);
            SET @OverdraftAmount = @Amount - @CurrentBalance;

            UPDATE tbl_Accounts
            SET account_balance = 0,
                overdraft = (overdraft - @Amount)-1.25
            WHERE account_id = @accountFrom;

            -- Insert into Payment table
            INSERT INTO tbl_payments (Bill_id, Amount, Date, account_id, user_id)
            VALUES (@Bill_id, @Amount, GETDATE(), @account_id, @user_id);

            SET @Payment_id = SCOPE_IDENTITY(); -- Get the generated Payment_id

            -- Insert into Transaction table (payment as credit to accountFrom and debit to accountTo)
            INSERT INTO tbl_Transactions (User_id, accountFrom, accountTo, Transaction_type, Amount, Date)
            VALUES (@user_id, @accountFrom, @Bill_id, 'Payment', @Amount, GETDATE());

            -- Insert into Transaction table (overdraft)
            INSERT INTO tbl_Transactions (User_id, accountFrom, accountTo, Transaction_type, Amount, Date)
            VALUES (@user_id, @accountFrom, @accountFrom, 'Overdraft', @OverdraftAmount, GETDATE());
			SET @Result = 0;
            COMMIT; -- Commit the transaction

            RETURN 0; -- Payment successful with overdraft
        END
        ELSE
        BEGIN
            ROLLBACK; -- Rollback the transaction
            RAISERROR ('Payment failed. Insufficient funds.', 16, 1); -- Raise an error
        END
    END
END;
