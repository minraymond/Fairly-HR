using Sabio.Models;
using Sabio.Models.Domain;
using Sabio.Models.Domain.Teams;
using Sabio.Models.Requests.Teams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Services.Interfaces
{
    public interface ITeamService
    {
        void DelTeamMembers(int userId, int teamId, int createdBy);

        void AddTeamMembers(TeamMembersAddRequest model, int userId);

        int Add(TeamAddRequest model, int CreatedBy);

        Team Get(int id);

        void Update(TeamUpdateRequest model);

        void Delete(int id);

        Paged<Team> GetOrgId(int orgId, int pageIndex, int pageSize);

        List<LookUp> GetTeamNamesByOrganizationId(int orgId);

    }
}
