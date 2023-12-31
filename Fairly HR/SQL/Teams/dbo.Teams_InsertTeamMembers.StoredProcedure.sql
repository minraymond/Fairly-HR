USE [Fairly]
GO
/****** Object:  StoredProcedure [dbo].[Teams_InsertTeamMembers]    Script Date: 6/12/2023 10:31:29 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author: Raymond Min
-- Create date: 06/06/2023
-- Description:	Insert Team Members for Teams
-- Code Reviewer: 
-- MODIFIED BY: 
-- MODIFIED DATE: 
-- Code Reviewer:
-- Note:
-- =============================================

CREATE PROCEDURE [dbo].[Teams_InsertTeamMembers]
						@UserId int,
						@TeamId int,
						@CreatedBy int

AS

/*

Declare @UserId int = 2
		,@TeamId int = 4
		,@CreatedBy int = 3

Execute [dbo].[Teams_InsertTeamMembers]
		@UserId
		,@TeamId
		,@CreatedBy
				

SELECT * 
FROM dbo.TeamMembers

*/

BEGIN

DECLARE @datenow datetime2 = getutcdate()

INSERT INTO dbo.TeamMembers
			(
				UserId
				,TeamId
				,CreatedBy
				,DateCreated
			)
		VALUES
			(
				@UserId
				,@TeamId
				,@CreatedBy
				,@datenow
				)


END
GO
