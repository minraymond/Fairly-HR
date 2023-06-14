using Sabio.Data.Providers;
using Sabio.Models.Requests.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Sabio.Services.Interfaces;
using System.Data;
using Sabio.Data;
using Sabio.Models;
using Sabio.Models.Domain.Locations;
using System.Reflection;
using Sabio.Models.Domain;
using Sabio.Models.Domain.Organizations;
using Sabio.Models.Domain.Newsletters;
using Sabio.Models.Domain.Jobs;

namespace Sabio.Services
{
    public class JobService : IJobService
    {
        private static IDataProvider _data = null;
        private static ILookUpService _lookUpService = null;

        public JobService(IDataProvider data, ILookUpService lookUpService)
        {
            _data = data;
            _lookUpService = lookUpService;
        }
     
        public void UpdateJobStatus(JobUpdateStatusRequest model)
        {
            string procName = "[dbo].[Jobs_Update_Status]";
            _data.ExecuteNonQuery(procName,
               inputParamMapper: delegate (SqlParameterCollection collection)
               {

                   collection.AddWithValue("@Id", model.Id);

               },
               returnParameters: null);
        }

        public void Update(JobUpdateRequest model, int userId)
        {
            string procName = "[dbo].[Jobs_Update]";
            _data.ExecuteNonQuery(procName,
               inputParamMapper: delegate (SqlParameterCollection collection)
               {
                   AddCommonParams(model, collection, userId);
                   collection.AddWithValue("@Id", model.Id);

               },
               returnParameters: null);
        }

        public Job GetById(int id)
        {
            string procName = "[dbo].[Jobs_SelectById]";
            Job aJob = null;

            _data.ExecuteCmd(procName, delegate (SqlParameterCollection parameterCollection)

            {
                parameterCollection.AddWithValue("@Id", id);

            }, delegate (IDataReader reader, short set)
            {
                int index = 0;

                aJob = MapSingleJob(reader, ref index);
            }
            );
            return aJob;
        }

        public int Add(JobAddRequest model, int userId)
        {
            int id = 0;

            string procName = "[dbo].[Jobs_Insert_V2]";

            _data.ExecuteNonQuery(procName,
               inputParamMapper: delegate (SqlParameterCollection collection)
               {
                   AddCommonParams(model, collection, userId);
                   collection.AddWithValue("@CreatedBy", userId);

                   SqlParameter idOut = new SqlParameter("@Id", SqlDbType.Int);
                   idOut.Direction = ParameterDirection.Output;

                   collection.Add(idOut);
               },
               returnParameters: delegate (SqlParameterCollection returnCollection)
               {
                   object oId = returnCollection["@Id"].Value;

                   int.TryParse(oId.ToString(), out id);            

               });

            return id;
        }

        public Paged<Job> GetOrgIdPaginated(int pageIndex, int pageSize, int organizationId)
        {
            Paged<Job> pagedList = null;
            List<Job> list = null;
            int totalCount = 0;

            _data.ExecuteCmd(
            "[dbo].[Jobs_SelectByOrgId_Paginated]",
            (param) =>
            {
                param.AddWithValue("@PageIndex", pageIndex);
                param.AddWithValue("@PageSize", pageSize);
                param.AddWithValue("@OrganizationId", organizationId);
            },
            (reader, recordeSetIndex) =>
            {
                int startIndex = 0;
                Job aJob = MapSingleJob(reader, ref startIndex);

                if (totalCount == 0)
                {
                    totalCount = reader.GetSafeInt32(startIndex++);
                }

                if (list == null)
                {
                    list = new List<Job>();
                }
                list.Add(aJob);
            }
            );
            if (list != null)
            {
                pagedList = new Paged<Job>(list, pageIndex, pageSize, totalCount);
            }
            return pagedList;
        }

        private static void AddCommonParams(JobAddRequest model, SqlParameterCollection collection, int currentUserId)
        {
            collection.AddWithValue("@Title", model.Title);
            collection.AddWithValue("@Description", model.Description);
            collection.AddWithValue("@Requirements", model.Requirements);
            collection.AddWithValue("@JobTypeId", model.JobTypeId);
            collection.AddWithValue("@JobStatusId", model.JobStatusId);
            collection.AddWithValue("@OrganizationId", model.OrganizationId);
            collection.AddWithValue("@LocationId", model.LocationId);
            collection.AddWithValue("@RemoteStatusId", model.RemoteStatusId);
            collection.AddWithValue("@ContactName", model.ContactName);
            collection.AddWithValue("@ContactPhone", model.ContactPhone);
            collection.AddWithValue("@ContactEmail", model.ContactEmail);
            collection.AddWithValue("@EstimatedStartDate", model.EstimatedStartDate);
            collection.AddWithValue("@EstimatedFinishDate", model.EstimatedFinishDate);
            collection.AddWithValue("@ModifiedBy", currentUserId);
        }

        public void AddJobLoc(JobLocAddRequest model, int userId)
        {
            string procName = "[dbo].[JobLocations_InsertV2]";
            _data.ExecuteNonQuery(procName,
                inputParamMapper: delegate (SqlParameterCollection col)
                {
                    col.AddWithValue("@JobId", model.JobId);
                    col.AddWithValue("@LocationId", model.LocationId);
                    col.AddWithValue("@CreatedBy", userId);

                }, returnParameters: null);

        }
        
        public JobLocations GetJobLoc(int jobId)
        {
            JobLocations jobLoc = null;

            string procName = "[dbo].[JobLocations_Select_ByJobId]";

            _data.ExecuteCmd(procName, delegate (SqlParameterCollection paramCollection)
            {

                paramCollection.AddWithValue("@JobId", jobId);

            }, delegate (IDataReader reader, short set)
            {
                int startingIndex = 0;
                jobLoc = MapSingleJobLoc(reader, ref startingIndex);

            });

            return jobLoc;
        }

        public void DeleteJobLoc(int jobId, int LocationId)
        {
            string procName = "[dbo].[JobLocations_Delete]";
            _data.ExecuteNonQuery(procName, delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@JobId", jobId);
                paramCollection.AddWithValue("@LocationId", LocationId);
            },

            returnParameters: null);
        }

        public Paged<Job> GetJobLocPage(int pageIndex, int pageSize, int locationId)
        {
            Paged<Job> pagedList = null;
            List<Job> list = null;
            int totalCount = 0;

            _data.ExecuteCmd("dbo.JobLocations_Select_ByLocationId_Range",
                (param) =>
                {
                    param.AddWithValue("@PageIndex", pageIndex);
                    param.AddWithValue("@PageSize", pageSize);
                    param.AddWithValue("@LocationId", locationId);

                },
                (reader, recordSetIndex) =>
                {
                    int startingIndex = 0;
                    Job jobLoc = MapSingleJob(reader, ref startingIndex);
                    if (totalCount == 0)
                    {
                        totalCount = reader.GetSafeInt32(startingIndex++);
                    }
                    if (list == null)
                    {
                        list = new List<Job>();
                    }

                    list.Add(jobLoc);
                }
                );
            if (list != null)
            {
                pagedList = new Paged<Job>(list, pageIndex, pageSize, totalCount);
            }
            return pagedList;
        }

        private static JobLocations MapSingleJobLoc(IDataReader reader, ref int startingIndex)
        {
            JobLocations jobLoc = new JobLocations();

            jobLoc.JobId = reader.GetSafeInt32(startingIndex++);
            jobLoc.Title = reader.GetSafeString(startingIndex++);
            jobLoc.Description = reader.GetSafeString(startingIndex++);
            jobLoc.Requirements = reader.GetSafeString(startingIndex++);
            jobLoc.ContactName = reader.GetSafeString(startingIndex++);
            jobLoc.ContactPhone = reader.GetSafeString(startingIndex++);
            jobLoc.ContactEmail = reader.GetSafeString(startingIndex++);
            jobLoc.EstimatedStartDate = reader.GetSafeDateTime(startingIndex++);
            jobLoc.EstimatedFinishDate = reader.GetSafeDateTime(startingIndex++);
            jobLoc.DateCreated = reader.GetSafeDateTime(startingIndex++);
            jobLoc.Locations = reader.DeserializeObject<List<Location>>(startingIndex++);

            return jobLoc;

        }

        public Paged<Job> GetAllPaginated(int pageIndex, int pageSize)
        {
            Paged<Job> pagedList = null;
            List<Job> list = null;
            int totalCount = 0;

            string procName = "dbo.Jobs_SelectAll_Paginated";

            _data.ExecuteCmd(procName,
                inputParamMapper: delegate (SqlParameterCollection parameterCollection)
                {
                    parameterCollection.AddWithValue("@PageIndex", pageIndex);
                    parameterCollection.AddWithValue("@PageSize", pageSize);
                },
                singleRecordMapper: delegate (IDataReader reader, short set)
                {
                    int startIndex = 0;
                    Job aJob = MapSingleJob(reader, ref startIndex);

                    if (totalCount == 0)
                    {
                        totalCount = reader.GetSafeInt32(startIndex++);
                    }

                    if (list == null)
                    {
                        list = new List<Job>();
                    }

                    list.Add(aJob);
                }
                );
            if (list != null)
            {
                pagedList = new Paged<Job>(list, pageIndex, pageSize, totalCount);
            }
            return pagedList;
        }

        public Paged<Job> GetByLocation(int pageIndex, int pageSize, double latitude, double longitude, int radius)
        {
            Paged<Job> pagedList = null;
            List<Job> list = null;
            int totalCount = 0;

            string procName = "dbo.Jobs_SelectByLocation";

            _data.ExecuteCmd(procName, inputParamMapper: delegate (SqlParameterCollection parameterCollection)
            {
                parameterCollection.AddWithValue("@PageIndex", pageIndex);
                parameterCollection.AddWithValue("@PageSize", pageSize);
                parameterCollection.AddWithValue("@Latitude", latitude);
                parameterCollection.AddWithValue("@Longitude", longitude);
                parameterCollection.AddWithValue("@Radius", radius);
            },
            singleRecordMapper: delegate (IDataReader reader, short set)
            {
                int startIndex = 0;
                Job aJob = MapSingleJob(reader, ref startIndex);

                if (totalCount == 0)
                {
                    totalCount = reader.GetSafeInt32(startIndex++);
                }

                if (list == null)
                {
                    list = new List<Job>();
                }

                list.Add(aJob);
            }
            );
            if (list != null)
            {
                pagedList = new Paged<Job>(list, pageIndex, pageSize, totalCount);
            }
            return pagedList;
        }

        public Paged<Job> SearchPaginated(int pageIndex, int pageSize, string query)
        {
            Paged<Job> pagedList = null;
            List<Job> list = null;
            int totalCount = 0;

            string procName = "dbo.Jobs_Search_Paginated";

            _data.ExecuteCmd(procName,
                inputParamMapper: delegate (SqlParameterCollection parameterCollection)
                {
                    parameterCollection.AddWithValue("@PageIndex", pageIndex);
                    parameterCollection.AddWithValue("@PageSize", pageSize);
                    parameterCollection.AddWithValue("@Query", query);
                },
                singleRecordMapper: delegate (IDataReader reader, short set)
                {
                    int startIndex = 0;
                    Job aJob = MapSingleJob(reader, ref startIndex);

                    if (totalCount == 0)
                    {
                        totalCount = reader.GetSafeInt32(startIndex++);
                    }

                    if (list == null)
                    {
                        list = new List<Job>();
                    }

                    list.Add(aJob);
                }
                );
            if (list != null)
            {
                pagedList = new Paged<Job>(list, pageIndex, pageSize, totalCount);
            }
            return pagedList;
        }

        private static Job MapSingleJob(IDataReader reader, ref int startingIndex)
        {
            Job aJob = new Job();

            aJob.Id = reader.GetSafeInt32(startingIndex++);
            aJob.Title = reader.GetSafeString(startingIndex++);
            aJob.Description = reader.GetSafeString(startingIndex++);
            aJob.Requirements = reader.GetSafeString(startingIndex++);
            aJob.JobType = _lookUpService.MapSingleLookUp(reader, ref startingIndex);
            aJob.JobStatus = _lookUpService.MapSingleLookUp(reader, ref startingIndex);

            aJob.Organization = new BaseOrganization();
            aJob.Organization.Id = reader.GetSafeInt32(startingIndex++);
            aJob.Organization.Name = reader.GetSafeString(startingIndex++);
            aJob.Organization.Headline = reader.GetSafeString(startingIndex++);
            aJob.Organization.Description = reader.GetSafeString(startingIndex++);
            aJob.Organization.Logo = reader.GetSafeString(startingIndex++);
            aJob.Organization.Phone = reader.GetSafeString(startingIndex++);
            aJob.Organization.SiteUrl = reader.GetSafeString(startingIndex++);

            aJob.Location = new Location();
            aJob.Location.Id = reader.GetSafeInt32(startingIndex++);
            aJob.Location.LineOne = reader.GetSafeString(startingIndex++);
            aJob.Location.LineTwo = reader.GetSafeString(startingIndex++);
            aJob.Location.City = reader.GetSafeString(startingIndex++);
            aJob.Location.Zip = reader.GetSafeString(startingIndex++);
            aJob.Location.Latitude = reader.GetSafeDouble(startingIndex++);
            aJob.Location.Longitude = reader.GetSafeDouble(startingIndex++);
            aJob.Location.LocationType = _lookUpService.MapSingleLookUp(reader, ref startingIndex);
            aJob.Location.State = _lookUpService.MapLookUp3Col(reader, ref startingIndex);
            aJob.Location.DateCreated = reader.GetSafeDateTime(startingIndex++);
            aJob.Location.DateModified = reader.GetSafeDateTime(startingIndex++);
            aJob.RemoteStatus = _lookUpService.MapSingleLookUp(reader, ref startingIndex);
            aJob.ContactName = reader.GetSafeString(startingIndex++);
            aJob.ContactPhone = reader.GetSafeString(startingIndex++);
            aJob.ContactEmail = reader.GetSafeString(startingIndex++);
            aJob.EstimatedStartDate = reader.GetSafeDateTime(startingIndex++);
            aJob.EstimatedFinishDate = reader.GetSafeDateTime(startingIndex++);
            aJob.DateCreated = reader.GetSafeDateTime(startingIndex++);
            aJob.DateModified = reader.GetSafeDateTime(startingIndex++);
            aJob.CreatedBy = reader.GetSafeInt32(startingIndex++);
            aJob.ModifiedBy = reader.GetSafeInt32(startingIndex++);

            return aJob;
        }

    }
}

