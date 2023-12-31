USE [Fairly]
GO
/****** Object:  StoredProcedure [dbo].[Teams_Insert]    Script Date: 5/19/2023 2:45:16 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author: Raymond Min
-- Create date: 05/09/2023
-- Description:	Insert for Teams
-- Code Reviewer: Michael Sanchez
-- MODIFIED BY: 
-- MODIFIED DATE: 
-- Code Reviewer:
-- Note:
-- =============================================

CREATE PROCEDURE [dbo].[Teams_Insert]
						@OrganizationId int,
						@Name nvarchar(100),
						@Description nvarchar(500),
						@CreatedBy int,
						@Id int OUTPUT

AS

/*

Declare @Id int = 0
		,@OrganizationId int = 3
		,@Name nvarchar(100) = 'DevOps team'
		,@Description nvarchar(500) = 'Managing Databases'
		,@CreatedBy int = 3

Execute [dbo].[Teams_Insert]
					@OrganizationId
					,@Name
					,@Description
					,@CreatedBy 
					,@Id

SELECT * 
FROM dbo.Teams

*/

BEGIN

DECLARE @datenow datetime2 = getutcdate()




INSERT INTO [dbo].[Teams]
           (
            [OrganizationId]
           ,[Name]
           ,[Description]
           ,[CreatedBy]
		   ,[DateCreated]

		   )
     VALUES
           (@OrganizationId
           ,@Name
           ,@Description
           ,@CreatedBy
		   ,@datenow
		   )
SET @Id = SCOPE_IDENTITY()

INSERT INTO dbo.TeamMembers
			(
				UserId
				,TeamId
				,CreatedBy
				,DateCreated
			)
		VALUES
			(
				@CreatedBy
				,@Id
				,@CreatedBy
				,@datenow
				)
END
GO
