USE [Fairly]
GO
/****** Object:  StoredProcedure [dbo].[TeamMembers_SelectAll]    Script Date: 5/2/2023 10:48:10 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/****** Script for SelectTopNRows command from SSMS  ******/
-- =============================================
-- Author: Rasean Rhone
-- Create date: 05/1/2023
-- Description:	Selecting team members from appointments.
-- Code Reviewer: Andy Chipres
-- MODIFIED BY: 
-- MODIFIED DATE: 
-- Code Reviewer:
-- Note:
-- =============================================
CREATE PROCEDURE [dbo].[TeamMembers_SelectAll]

AS

/*

Execute [dbo].[TeamMembers_SelectAll]

*/

BEGIN

		SELECT  T.[UserId]
				,U.FirstName + ' ' + U.LastName as Name

		FROM	[dbo].[TeamMembers] as T
				INNER JOIN dbo.Users as U on U.Id = T.[UserId]

END



GO
