USE [AtmProject]
GO
/****** Object:  StoredProcedure [dbo].[CreateUserWithAccounts]    Script Date: 8/29/2023 3:34:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[CreateUserWithAccounts]
(
    @NIP VARCHAR(20),
    @Nom VARCHAR(50),
    @Prenom VARCHAR(50),
    @Telephone VARCHAR(20),
    @Email VARCHAR(100),
    @Role VARCHAR(50),
    @Status VARCHAR(50)
)
AS
BEGIN
    DECLARE @UserID INT

    -- Insert into Users table
    INSERT INTO tbl_Users (NIP, Nom, Prenom, Telephone, Email, Role, Status)
    VALUES (@NIP, @Nom, @Prenom, @Telephone, @Email, @Role, @Status)

    SET @UserID = SCOPE_IDENTITY()  -- Get the auto-generated User ID

    -- Insert Checking account for the user
    INSERT INTO tbl_Accounts (account_type, account_balance, status, User_id, overdraft_enabled, overdraft)
    VALUES ('Checking', 0, 1, @UserID, 0, 0)

    -- Insert Saving account for the user
    INSERT INTO tbl_Accounts (account_type, account_balance, status, User_id, overdraft_enabled, overdraft)
    VALUES ('Saving', 0, 1, @UserID, 0, 0)
END
