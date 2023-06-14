using Sabio.Models.Domain.Locations;
using Sabio.Models.Domain.Organizations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Models.Domain.Jobs
{
    public class BaseJob
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public LookUp Type { get; set; }
        public LookUp JobStatus { get; set; }
        public LookUp RemoteStatus { get; set; }
    }
}
