USE [Fairly]
GO
/****** Object:  StoredProcedure [dbo].[Teams_SelectByOrgId]    Script Date: 5/19/2023 2:45:16 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author: Raymond Min
-- Create date: 05/09/2023
-- Description:	SelectByOrgId for Teams Paginated
-- Code Reviewer: Michael Sanchez
-- MODIFIED BY: 
-- MODIFIED DATE: 
-- Code Reviewer:
-- Note:
-- =============================================



CREATE PROC [dbo].[Teams_SelectByOrgId]
		@OrganizationId int,
		@PageIndex int,
		@PageSize int
as

/* ---------------TEST-------------
	 
	DECLARE @OrganizationId int = 1,
			@PageIndex int = 0,
			@PageSize int = 10

	EXECUTE [dbo].[Teams_SelectByOrgId]
		@OrganizationId,
		@PageIndex,
		@PageSize

*/

BEGIN

DECLARE @Offset int = @PageIndex * @PageSize

		SELECT
		t.[Id]
		,OrganizationId
		,o.[Name] as OrgName
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
		,TotalCount = COUNT(1) OVER()				



	FROM [dbo].[Teams] as t
	inner join dbo.Organizations as o on o.Id = t.OrganizationId
	

	ORDER BY t.OrganizationId
	OFFSET @OffSet ROWS
	FETCH NEXT @PageSize ROWS ONLY




END
GO
