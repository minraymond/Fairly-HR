using Sabio.Models.Domain.Organizations;
using Sabio.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Models.Domain.Teams
{
    public class Team
    {
        public BaseOrganization Organization { get; set; }
        public string TeamName { get; set; }
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public List<TeamMembers> TeamMember { get; set; }

    }
}
