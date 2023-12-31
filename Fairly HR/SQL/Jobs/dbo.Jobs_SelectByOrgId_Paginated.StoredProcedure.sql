USE [Fairly]
GO
/****** Object:  StoredProcedure [dbo].[Jobs_SelectByOrgId_Paginated]    Script Date: 6/2/2023 8:57:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Author: RAYMOND MIN
-- Create date: <05/27/2023>
-- Description: <Fairly Jobs SelectByOrgId Paginate Proc>
-- Code Reviewer:

-- MODIFIED BY: 
-- MODIFIED DATE:12/1/2020
-- Code Reviewer:
-- Note:


CREATE PROC [dbo].[Jobs_SelectByOrgId_Paginated]
		@PageIndex int,
		@PageSize int, 
		@OrganizationId int


as


/*

Declare @PageIndex int = 0
		,@PageSize int = 2
		,@OrganizationId int = 1

execute [dbo].[Jobs_SelectByOrgId_Paginated]

		 @PageIndex 
		,@PageSize 
		,@OrganizationId 


*/


BEGIN

DECLARE	@offset int = @PageIndex * @PageSize

SELECT 	j.Id
	  ,j.Title
	  ,j.[Description]
	  ,j.Requirements
	  ,jt.Id as JobTypeId
	  ,jt.[Name] as JobTypeName
	  ,js.Id as JobStatusId
	  ,js.[Name] as JobStatusName
	  ,o.Id as OrganizationId
	  ,o.[Name] as Company
	  ,o.Headline
	  ,o.[Description]
	  ,o.Logo
	  ,o.Phone
	  ,o.SiteUrl
	  ,l.Id as LocationId
	  ,l.LineOne
	  ,l.LineTwo
	  ,l.City
	  ,l.Zip
	  ,l.Latitude
	  ,l.Longitude
	  ,LocationTypeId = (Select lt.[Id]
						 From dbo.LocationTypes as lt
						 Where lt.Id = l.LocationTypeId)
	  ,LocationTypeName = (Select lt.[Name]
						 From dbo.LocationTypes as lt
						 Where lt.Id = l.LocationTypeId)
	  ,s.Id
	  ,s.[Name]
	  ,s.Code
	  ,l.DateCreated
	  ,l.DateModified
	  ,rs.[Id] as RemoteStatusId
	  ,rs.[Name]
      ,j.[ContactName]
      ,j.[ContactPhone]
      ,j.[ContactEmail]
      ,j.[EstimatedStartDate]
      ,j.[EstimatedFinishDate]
      ,j.[DateCreated]
      ,j.[DateModified]
      ,j.[CreatedBy]
      ,j.[ModifiedBy]
	  ,TotalCount = Count(1) OVER()
	FROM dbo.Jobs as j inner join dbo.Locations as l
	on j.LocationId = l.Id
	inner join dbo.JobStatus as js
	on j.JobStatusId = js.Id
	inner join dbo.JobTypes as jt
	on j.JobTypeId = jt.Id
	inner join dbo.Organizations as o
	on j.OrganizationId = o.Id
	inner join dbo.States as s
	on l.StateId = s.Id
	inner join dbo.RemoteStatus as rs
	on j.RemoteStatusId = rs.Id

	Where o.Id = @OrganizationId
			

  Order by j.OrganizationId
  offset @offset rows
  fetch next @PageSize rows only


END

GO
