USE [AtmProject]
GO
/****** Object:  StoredProcedure [dbo].[openATM]    Script Date: 8/29/2023 3:35:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Create the stored procedure
ALTER PROCEDURE [dbo].[openATM]
AS
BEGIN
    -- Update the ATM_Open column to false
    UPDATE tbl_close_atm
    SET ATM_Open = 1;
END;
