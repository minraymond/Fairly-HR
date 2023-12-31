USE [Fairly]
GO
/****** Object:  StoredProcedure [dbo].[Teams_SelectById]    Script Date: 5/19/2023 2:45:16 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author: Raymond Min
-- Create date: 05/09/2023
-- Description:	SelectById for Teams
-- Code Reviewer: 
-- MODIFIED BY: 
-- MODIFIED DATE: 
-- Code Reviewer:
-- Note:
-- =============================================



CREATE PROC [dbo].[Teams_SelectById]
		@Id int
as

/* ---------------------TEST--------------------
	Declare @Id int = 4

	Execute [dbo].[Teams_SelectById] @Id


*/

BEGIN

--Teams Table Column Id == TeamMembers Table Column TeamId
--Teams Table Column CreatedBy == Users Table Column Id
--TeamMembers Table Column == Users Table Column UserId


	SELECT	
		t.[Id]
		,OrganizationId
		,o.[Name] as [OrgName]
		,t.[Name] as TeamName
		,t.[Description]
		,t.[DateCreated]
		,t.[DateModified]
		,TeamMembers = 
			(
			SELECT
				u.[id]	as userId
				,u.[email]
				,u.FirstName 
				,u.Mi  
				,u.LastName
				,u.AvatarUrl
			FROM dbo.Users as u 
			INNER JOIN dbo.TeamMembers as tm 
					on u.Id = tm.UserId
			WHERE tm.TeamId = t.Id
			FOR JSON AUTO
			)
								
	FROM [dbo].[Teams] as t
	inner join dbo.Organizations as o on o.Id = t.OrganizationId
	WHERE t.Id = @Id


END
GO
