USE [AtmProject]
GO
/****** Object:  StoredProcedure [dbo].[AddPaperFunds]    Script Date: 8/28/2023 5:12:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[AddPaperFunds]
AS
BEGIN
    DECLARE @CurrentPaperFunds BIGINT;
    
    -- Get the current paperFunds value
    SELECT @CurrentPaperFunds = paperFunds FROM tbl_paperFunds;
    
    -- Check if paperFunds is less than or equal to 5000
    IF @CurrentPaperFunds <= 5000
    BEGIN
        -- Add 15000 to paperFunds
        UPDATE tbl_paperFunds
        SET paperFunds = paperFunds + 15000;
        
        -- Insert transaction record into the Transaction table
        INSERT INTO tbl_Transactions ([User_id], [accountFrom], [accountTo], [Transaction_type], [Amount], [Date])
        VALUES (Null, Null, 'Paper Funds', 'Deposit', 15000, GETDATE());
        
        PRINT 'Paper funds added successfully.';
    END
    ELSE
    BEGIN
        PRINT 'Transaction not allowed. Paper funds are greater than 5000.';
    END
END
