USE [Fairly]
GO
/****** Object:  StoredProcedure [dbo].[Teams_SelectAll]    Script Date: 5/19/2023 2:45:16 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author: Raymond Min
-- Create date: 05/09/2023
-- Description:	SelectAll for Teams filtered by OrgId
-- Code Reviewer: Michael Sanchez
-- MODIFIED BY: 
-- MODIFIED DATE: 
-- Code Reviewer:
-- Note:
-- =============================================



CREATE PROC [dbo].[Teams_SelectAll]
	@OrganizationId int
as

/* ---------------TEST-------------
	 
	DECLARE @OrganizationId int = 3

	Execute dbo.Teams_SelectAll @OrganizationId


	Select *
	from dbo.Teams

	Select *
	from dbo.Organizations

*/

BEGIN




	SELECT Id
		,Name
			 

	FROM dbo.Teams 
	WHERE OrganizationId = @OrganizationId






END
GO
