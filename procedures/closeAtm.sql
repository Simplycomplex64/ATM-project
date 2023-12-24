USE [AtmProject]
GO
/****** Object:  StoredProcedure [dbo].[CloseATM]    Script Date: 8/29/2023 3:33:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Create the stored procedure
ALTER PROCEDURE [dbo].[CloseATM]
AS
BEGIN
    -- Update the ATM_Open column to false
    UPDATE tbl_close_atm
    SET ATM_Open = 0;
END;
