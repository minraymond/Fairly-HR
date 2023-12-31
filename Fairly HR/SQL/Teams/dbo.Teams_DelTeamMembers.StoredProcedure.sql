USE [Fairly]
GO
/****** Object:  StoredProcedure [dbo].[Teams_DelTeamMembers]    Script Date: 6/14/2023 1:22:17 PM ******/
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

CREATE PROCEDURE [dbo].[Teams_DelTeamMembers]
						@UserId int,
						@TeamId int,
						@CreatedBy int

AS

/*

Declare @UserId int = 222
		,@TeamId int = 40
		,@CreatedBy int = 222

Execute [dbo].[Teams_DelTeamMembers]
		@UserId
		,@TeamId
		,@CreatedBy
				

SELECT * 
FROM dbo.TeamMembers

*/

BEGIN

DECLARE @datenow datetime2 = getutcdate()

DELETE FROM dbo.TeamMembers
where UserId = @UserId and TeamId = @TeamId


END
GO
