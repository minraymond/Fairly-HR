USE [Fairly]
GO
/****** Object:  StoredProcedure [dbo].[Teams_Delete]    Script Date: 5/19/2023 2:45:16 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author: Raymond Min
-- Create date: 05/09/2023
-- Description:	Delete by Id for Teams
-- Code Reviewer: 
-- MODIFIED BY: 
-- MODIFIED DATE: 
-- Code Reviewer:
-- Note:
-- =============================================



CREATE PROC [dbo].[Teams_Delete]
		@Id int


as

/* ---------------TEST-------------
	Declare @Id int = 4
	
	 Execute [dbo].[Teams_SelectById] @Id
	 SELECT * FROM dbo.TeamMembers

	 Execute [dbo].[Teams_Delete] @Id


	 Execute [dbo].[Teams_SelectById] @Id
	 SELECT * FROM dbo.TeamMembers


*/

BEGIN

DELETE FROM [dbo].[TeamMembers]
WHERE TeamId = @Id

DELETE FROM [dbo].[Teams]
WHERE Id = @Id




END
GO
