using Sabio.Models;
using Sabio.Models.Domain;
using Sabio.Models.Domain.Jobs;
using Sabio.Models.Requests.Jobs;
using System.Collections.Generic;

namespace Sabio.Services.Interfaces
{
    public interface IJobService
    {
        Job GetById(int id);
        int Add(JobAddRequest model, int userId);
        void Update(JobUpdateRequest model, int userId);
        void UpdateJobStatus(JobUpdateStatusRequest model);
        Paged<Job> GetOrgIdPaginated(int pageIndex, int pageSize, int organizationId);
        void DeleteJobLoc(int jobId, int locationId);
        JobLocations GetJobLoc(int jobId);
        Paged<Job> GetJobLocPage(int pageIndex, int pageSize, int locationId);
        void AddJobLoc(JobLocAddRequest model, int userId);
        Paged<Job> GetAllPaginated(int pageIndex, int pageSize);
        Paged<Job> SearchPaginated(int pageIndex, int pageSize, string query);
        Paged<Job> GetByLocation(int pageIndex, int pageSize, double latitude, double longitude, int radius);
    }
}

